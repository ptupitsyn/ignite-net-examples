using System;
using Apache.Ignite.Core;

namespace IgnitePlugin
{
    internal static class Program
    {
        private static void Main()
        {
            var cfg = new IgniteConfiguration
            {
                JvmClasspath = @"..\..\..\..\Java\target\IgniteNetSemaphorePlugin-1.0-SNAPSHOT.jar",
                PluginConfigurations = new[] {new SemaphorePluginConfiguration()}
            };

            var ignite = Ignition.Start(cfg);
            var sem = ignite.GetOrCreateSemaphore("foo", 2);

            Console.WriteLine();
            Console.WriteLine("Trying to acquire semaphore...");

            sem.WaitOne();

            Console.WriteLine("Semaphore acquired. Press any key to release.");
            Console.ReadKey();

            sem.Release();
        }
    }
}
