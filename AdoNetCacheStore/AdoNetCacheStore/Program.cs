using System;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Binary;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;

namespace AdoNetCacheStore
{
    class Program
    {
        static void Main()
        {
            AdoNetCacheStore.InitializeDb();

            using (var ignite = Ignition.StartFromApplicationConfiguration())
            {
                var cacheCfg = new CacheConfiguration
                {
                    Name = "cars",
                    KeepBinaryInStore = true,
                    CacheStoreFactory = new AdoNetCacheStoreFactory(),
                    ReadThrough = true,
                    WriteThrough = true
                };

                ICache<int, IBinaryObject> cars = ignite.GetOrCreateCache<int, object>(cacheCfg).WithKeepBinary<int, IBinaryObject>();

                IBinaryObject car = ignite.GetBinary()
                    .GetBuilder("Car")
                    .SetStringField("Name", "Honda NSX")
                    .SetIntField("Power", 600)
                    .Build();

                // Put an entry to Ignite cache, this causes AdoNetCacheStore.Write call.
                cars.Put(1, car);

                // Remove an entry from Ignite cache, but not from cache store.
                Console.WriteLine("Cache size before Clear: " + cars.GetSize());
                cars.Clear();
                Console.WriteLine("Cache size after Clear: " + cars.GetSize());

                // Get an entry by key, which delegates to AdoNetCacheStore.Load.
                Console.WriteLine("Requesting key from Ignite cache...");
                IBinaryObject carFromStore = cars.Get(1);
                Console.WriteLine("Entry from cache store: " + carFromStore);
            }
        }
    }
}
