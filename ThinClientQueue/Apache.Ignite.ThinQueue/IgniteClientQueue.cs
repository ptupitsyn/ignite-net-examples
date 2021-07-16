using System;
using System.Collections.Generic;
using System.Threading;
using Apache.Ignite.Core.Cache.Event;
using Apache.Ignite.Core.Client;
using Apache.Ignite.Core.Client.Cache;
using Apache.Ignite.Core.Client.Cache.Query.Continuous;
using Apache.Ignite.Core.Common;
using Apache.Ignite.Core.Impl.Common;

namespace Apache.Ignite.ThinQueue
{
    /// <summary>
    /// Distributed thin client Ignite queue.
    /// </summary>
    public sealed class IgniteClientQueue<T> : ICacheEntryEventListener<Guid, (T, Guid)>
    {
        private const int CounterId = -1;

        private readonly IIgniteClient _client;

        private readonly ICacheClient<Guid, (T Value, Guid Prev)> _cache;

        private readonly ICacheClient<int, (int Count, Guid Id)> _cacheCounter;

        private readonly object _querySyncRoot = new object();

        public IgniteClientQueue(IIgniteClient client, string name)
        {
            IgniteArgumentCheck.NotNull(client, nameof(client));
            IgniteArgumentCheck.Ensure(!string.IsNullOrEmpty(name), nameof(name), "Name should not be null or empty");

            _client = client;

            _cache = client.GetOrCreateCache<Guid, (T, Guid)>(name);

            // Use the same cache with different value type to store the ID counter.
            _cacheCounter = client.GetCache<int, (int, Guid)>(name);
            _cacheCounter.PutIfAbsent(CounterId, (0, Guid.NewGuid()));
        }

        public int Count => _cacheCounter[CounterId].Item1;

        public void Enqueue(T item)
        {
            while (true)
            {
                var count = _cacheCounter[CounterId];
                var newCount = (count.Count + 1, Id: Guid.NewGuid());

                if (_cacheCounter.Replace(key: CounterId, oldVal: count, newVal: newCount))
                {
                    _cache[newCount.Id] = (Value: item, Prev: count.Id);

                    return;
                }
            }
        }

        public bool TryDequeue(out T result)
        {
            while (true)
            {
                var count = _cacheCounter[CounterId];

                if (count.Count == 0)
                {
                    result = default;
                    return false;
                }

                if (!_cache.TryGet(count.Id, out var res))
                {
                    continue;
                }

                if (_cacheCounter.Replace(key: CounterId, oldVal: count, newVal: (count.Count - 1, res.Prev)))
                {
                    result = res.Value;
                    _cache.Remove(count.Id);

                    return true;
                }
            }
        }

        /// <summary>
        /// Dequeue or throw when empty.
        /// </summary>
        public T Dequeue() =>
            TryDequeue(out var result)
                ? result
                : throw new IgniteException("Queue is empty");

        /// <summary>
        /// Blocking dequeue: waits for an item to be available.
        /// </summary>
        public T Take()
        {
            if (TryDequeue(out var result))
            {
                return result;
            }

            lock (_querySyncRoot)
            {
                using var query = _cache.QueryContinuous(new ContinuousQueryClient<Guid, (T, Guid)>(this));

                while (true)
                {
                    if (TryDequeue(out result))
                    {
                        return result;
                    }

                    Monitor.Wait(_querySyncRoot);
                }
            }
        }

        public void Close()
        {
            try
            {
                _client.DestroyCache(_cache.Name);
            }
            catch (IgniteException e) when (e.Message.StartsWith("Cache does not exist", StringComparison.Ordinal))
            {
                // Ignore: already closed.
            }
        }

        void ICacheEntryEventListener<Guid, (T, Guid)>.OnEvent(IEnumerable<ICacheEntryEvent<Guid, (T, Guid)>> evts)
        {
            lock (_querySyncRoot)
            {
                Monitor.Pulse(_querySyncRoot);
            }
        }
    }
}
