using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Binary;
using Apache.Ignite.Core.Cache.Query;
using Apache.Ignite.Core.Client;
using IgniteMobileXamarinForms.Models;

namespace IgniteMobileXamarinForms.Services
{
    public class MockDataStore : IDataStore<Item>
    {
        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            var logger = new IgniteListLogger();

            var cfg = new IgniteClientConfiguration
            {
                Endpoints = new[] { "192.168.0.117:10800" },
                Logger = logger,
                SocketTimeout = TimeSpan.FromSeconds(3),
            };

            var res = new List<Item>();

            using (var ignite = Ignition.StartClient(cfg))
            {
                foreach (var cacheName in ignite.GetCacheNames())
                {
                    var cache = ignite.GetCache<object, object>(cacheName).WithKeepBinary<object, IBinaryObject>();
                    var size = await cache.GetSizeAsync();
                    var firstItem = cache.Query(new ScanQuery<object, IBinaryObject>()).FirstOrDefault();

                    var item = new Item
                    {
                        Id = cacheName,
                        Text = $"{cacheName} ({size} entries)",
                        Description = firstItem == null ? "<no data>" : firstItem.ToString()
                    };

                    res.Add(item);
                }
            }

            return res;
        }
    }
}