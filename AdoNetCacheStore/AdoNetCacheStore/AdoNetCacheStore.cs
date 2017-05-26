using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Binary;
using Apache.Ignite.Core.Cache.Store;
using Apache.Ignite.Core.Common;
using Apache.Ignite.Core.Resource;

namespace AdoNetCacheStore
{
    public class AdoNetCacheStore : ICacheStore<int, IBinaryObject>
    {
        public const string DbName = "cars.db";

        public const string ConnectionString = "DataSource=" + DbName;

        [InstanceResource]
        private IIgnite Ignite { get; set; }

        public static void InitializeDb()
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

        public void LoadCache(Action<int, IBinaryObject> act, params object[] args)
        {
            // No-op.
        }

        public IBinaryObject Load(int key)
        {
            using (var conn = new SqlCeConnection(ConnectionString))
            {
                using (var cmd = new SqlCeCommand(@"SELECT Name, Power FROM Cars WHERE Id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", key);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Ignite.GetBinary()
                                .GetBuilder("Car")
                                .SetStringField("Name", reader.GetString(0))
                                .SetIntField("Power", reader.GetInt32(1))
                                .Build();
                        }
                        
                        return null;
                    }
                }
            }
        }

        public IEnumerable<KeyValuePair<int, IBinaryObject>> LoadAll(IEnumerable<int> keys)
        {
            return keys.ToDictionary(x => x, Load);
        }

        public void Write(int key, IBinaryObject val)
        {
            throw new NotImplementedException();
        }

        public void WriteAll(IEnumerable<KeyValuePair<int, IBinaryObject>> entries)
        {
            foreach (var pair in entries)
            {
                Write(pair.Key, pair.Value);
            }
        }

        public void Delete(int key)
        {
            // TODO
        }

        public void DeleteAll(IEnumerable<int> keys)
        {
            foreach (var key in keys)
            {
                Delete(key);
            }
        }

        public void SessionEnd(bool commit)
        {
            // No-op.
        }
    }

    public class AdoNetCacheStoreFactory : IFactory<AdoNetCacheStore>
    {
        public AdoNetCacheStore CreateInstance()
        {
            return new AdoNetCacheStore();
        }
    }
}
