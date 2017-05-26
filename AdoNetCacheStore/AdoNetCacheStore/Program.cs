using System.ComponentModel;
using System.Data.SqlServerCe;
using System.IO;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Binary;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;

namespace AdoNetCacheStore
{
    class Program
    {
        public const string DbName = "cars.db";
        public const string ConnectionString = "DataSource=" + DbName;

        static void Main()
        {
            InitializeDb();

            using (var ignite = Ignition.StartFromApplicationConfiguration())
            {
                var cacheCfg = new CacheConfiguration
                {
                    Name = "cars",
                    KeepBinaryInStore = true,
                    CacheStoreFactory = new AdoNetCacheStoreFactory()
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

        private static void InitializeDb()
        {
            File.Delete(DbName);

            using (var engine = new SqlCeEngine(ConnectionString))
            {
                engine.CreateDatabase();
            }

            using (var conn = new SqlCeConnection(ConnectionString))
            {
                using (var cmd = new SqlCeCommand(@"CREATE TABLE Cars (ID int, Name NVARCHAR(200), Power int)", conn))
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
