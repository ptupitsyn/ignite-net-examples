using Apache.Ignite.Core;
using Apache.Ignite.Core.Deployment;
using ComputeClassLib;

namespace ComputeTest
{
    static class Program
    {
        private static void Main()
        {
            var cfg = new IgniteConfiguration
            {
                ClientMode = true,
                PeerAssemblyLoadingMode = PeerAssemblyLoadingMode.CurrentAppDomain
            };

            var client = Ignition.Start(cfg);
            client.GetCompute().Broadcast(new ConsoleWriteAction());
        }
    }
}
