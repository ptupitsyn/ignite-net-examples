using System;
using System.Threading;
using System.Threading.Tasks;
using Apache.Ignite.Core;

namespace IgnitePlugin
{
    static class Program
    {
        static void Main(string[] args)
        {
            var cfg = new IgniteConfiguration
            {
                JvmClasspath = @"..\..\..\..\Java\target\IgniteNetSemaphorePlugin-1.0-SNAPSHOT.jar",
                PluginConfigurations = new[] {new SemaphorePluginConfiguration()}
            };

            Ignition.Start(cfg);

            for (var i = 0; i < 10; i++)
            {
                var id = i;
                Task.Run(() => RunThread(id));
            }

            Console.ReadKey();
        }

        private static void RunThread(int id)
        {
            var sem = Ignition.GetIgnite().GetOrCreateSemaphore("foo", 2);

            sem.WaitOne();
            Console.WriteLine($"Thread {id} has entered semaphore.");
            Thread.Sleep(500); // Simulate work
            sem.Release();
            Console.WriteLine($"Thread {id} has left semaphore.");
        }
    }
}
