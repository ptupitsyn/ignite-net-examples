using Apache.Ignite.Core;

namespace IgnitePlugin
{
    class Program
    {
        static void Main(string[] args)
        {
            var cfg = new IgniteConfiguration
            {
                JvmClasspath = @"..\..\..\..\Java\target\IgniteNetSemaphorePlugin-1.0-SNAPSHOT.jar"
            };

            Ignition.Start(cfg);
        }
    }
}
