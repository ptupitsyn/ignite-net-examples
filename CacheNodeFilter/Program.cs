using System;
using Apache.Ignite.Core;

namespace CacheNodeFilter
{
    class Program
    {
        static void Main(string[] args)
        {
            // Start Ignite server with Users.
            var userServer = Ignition.Start(
                new IgniteConfiguration {SpringConfigUrl = "user-node-config.xml"});

            var userNode = userServer.GetCluster().GetLocalNode();

            // Start Ignite server with Companies.
            var companyServer = Ignition.Start(
                new IgniteConfiguration {SpringConfigUrl = "company-node-config.xml"});

            var companyNode = companyServer.GetCluster().GetLocalNode();

            // Start Client node to demonstrate data distribution.
            var client = Ignition.Start(
                new IgniteConfiguration {ClientMode = true, Localhost = "127.0.0.1"});

            var userPartitionsOnUserNode = client.GetAffinity("user").GetAllPartitions(userNode).Length;
            var userPartitionsOnCompanyNode = client.GetAffinity("user").GetAllPartitions(userNode).Length;
            
            Console.WriteLine($"User partitions on user node: {userPartitionsOnUserNode}, on company node: {userPartitionsOnCompanyNode}");
        }
    }
}
