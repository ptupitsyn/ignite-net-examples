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
    public sealed class IgniteClientQueue<T> : ICacheEntryEventListener<int, T>
    {
        private const int CounterId = -1;

        private readonly IIgniteClient _client;

        private readonly ICacheClient<int, T> _cache;

        private readonly ICacheClient<int, int> _cacheCounter;

        private readonly object _querySyncRoot = new object();

        public IgniteClientQueue(IIgniteClient client, string name)
        {
            IgniteArgumentCheck.NotNull(client, nameof(client));
            IgniteArgumentCheck.Ensure(!string.IsNullOrEmpty(name), nameof(name), "Name should not be null or empty");

            _client = client;

            _cache = client.GetOrCreateCache<int, T>(name);
            _cacheCounter = _cache.WithKeepBinary<int, int>();
            _cacheCounter.PutIfAbsent(CounterId, 0);
        }

        public int Count => _cacheCounter[CounterId];

        public void Enqueue(T item)
        {
            while (true)
            {
                var count = _cacheCounter[CounterId];

                if (_cacheCounter.Replace(key: CounterId, oldVal: count, newVal: count + 1))
                {
                    _cache[count] = item;

                    return;
                }
            }
        }

        public bool TryDequeue(out T result)
        {
            while (true)
            {
                var count = _cacheCounter[CounterId];

                if (count == 0)
                {
                    result = default;
                    return false;
                }

                if (_cacheCounter.Replace(key: CounterId, oldVal: count, newVal: count - 1))
                {
                    result = _cache.GetAndRemove(count - 1).Value;

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
                if (TryDequeue(out result))
                {
                    return result;
                }

                using var query = _cache.QueryContinuous(new ContinuousQueryClient<int, T>(this));

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

        void ICacheEntryEventListener<int, T>.OnEvent(IEnumerable<ICacheEntryEvent<int, T>> evts)
        {
            lock (_querySyncRoot)
            {
                Monitor.Pulse(_querySyncRoot);
            }
        }
    }
}
