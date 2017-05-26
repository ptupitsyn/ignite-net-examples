using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.IO;
using Apache.Ignite.Core.Cache.Store;
using Apache.Ignite.Core.Common;

namespace AdoNetCacheStore
{
    public class AdoNetCacheStore : ICacheStore<int, string>
    {
        public const string DbName = "cars.db";

        public const string ConnectionString = "DataSource=" + DbName;

        public void LoadCache(Action<int, string> act, params object[] args)
        {
            throw new NotImplementedException();
        }

        public string Load(int key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<int, string>> LoadAll(IEnumerable<int> keys)
        {
            throw new NotImplementedException();
        }

        public void Write(int key, string val)
        {
            throw new NotImplementedException();
        }

        public void WriteAll(IEnumerable<KeyValuePair<int, string>> entries)
        {
            throw new NotImplementedException();
        }

        public void Delete(int key)
        {
            throw new NotImplementedException();
        }

        public void DeleteAll(IEnumerable<int> keys)
        {
            throw new NotImplementedException();
        }

        public void SessionEnd(bool commit)
        {
            throw new NotImplementedException();
        }

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

    }

    public class AdoNetCacheStoreFactory : IFactory<AdoNetCacheStore>
    {
        public AdoNetCacheStore CreateInstance()
        {
            return new AdoNetCacheStore();
        }
    }
}
