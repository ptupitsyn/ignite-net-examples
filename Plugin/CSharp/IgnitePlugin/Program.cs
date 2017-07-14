using Apache.Ignite.Core;

namespace IgnitePlugin
{
    class Program
    {
        static void Main(string[] args)
        {
            var cfg = new IgniteConfiguration
            {
                JvmClasspath = @"..\..\..\..\Java\target\IgniteNetSemaphorePlugin-1.0-SNAPSHOT.jar",
                PluginConfigurations = new[] {new SemaphorePluginConfiguration()}
            };

            var ignite = Ignition.Start(cfg);

            var sem = ignite.GetOrCreateSemaphore("foo", 2);

            sem.WaitOne();
            sem.Release();
        }
    }
}
