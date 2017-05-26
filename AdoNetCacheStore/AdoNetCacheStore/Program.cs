using Apache.Ignite.Core;

namespace AdoNetCacheStore
{
    class Program
    {
        static void Main()
        {
            Ignition.StartFromApplicationConfiguration();
        }
    }
}
