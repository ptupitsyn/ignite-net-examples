using System;
using System.Threading;
using System.Threading.Tasks;
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
            var cluster = ignite.GetCluster();
            var nodeId = cluster.GetLocalNode().Order;

            // Wait for second node
            while (cluster.GetNodes().Count < 2)
            {
                Thread.Sleep(10);
            }

            for (var i = 0; i < 20; i++)
            {
                Thread.Sleep(300);

                var id = i;
                Task.Run(() => RunThread($"{nodeId}:{id}"));
            }

            Console.ReadKey();
        }

        private static void RunThread(string id)
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
