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

                cars[1] = car;
            }

        }
    }
}
