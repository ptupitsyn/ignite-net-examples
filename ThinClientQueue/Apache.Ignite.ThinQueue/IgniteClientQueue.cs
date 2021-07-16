using System;
using System.Collections.Generic;
using Apache.Ignite.Core.Cache.Event;
using Apache.Ignite.Core.Client;
using Apache.Ignite.Core.Client.Cache;
using Apache.Ignite.Core.Client.Cache.Query.Continuous;
using Apache.Ignite.Core.Impl.Common;

namespace Apache.Ignite.ThinQueue
{
    /// <summary>
    /// Distributed thin client Ignite queue.
    /// </summary>
    public sealed class IgniteClientQueue<T> : ICacheEntryEventListener<long, T>, IDisposable
    {
        private const long CounterId = -1;

        private readonly IIgniteClient _client;

        private readonly ICacheClient<long, T> _cache;

        private readonly ICacheClient<long, long> _cacheCounter;

        // private readonly IContinuousQueryHandleClient _queryHandle;

        public IgniteClientQueue(IIgniteClient client, string name)
        {
            IgniteArgumentCheck.NotNull(client, nameof(client));
            IgniteArgumentCheck.Ensure(!string.IsNullOrEmpty(name), nameof(name), "Name should not be null or empty");

            _client = client;

            _cache = client.GetOrCreateCache<long, T>(name);
            _cacheCounter = _cache.WithKeepBinary<long, long>();
            _cacheCounter.PutIfAbsent(CounterId, 0);

            // _queryHandle = _cache.QueryContinuous(new ContinuousQueryClient<long, T>(this));
        }

        public void OnEvent(IEnumerable<ICacheEntryEvent<long, T>> evts)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            _client.DestroyCache(_cache.Name);
        }

        public void Dispose()
        {
            // _queryHandle.Dispose();
            // _client.DestroyCache(_cache.Name);
        }
    }
}
