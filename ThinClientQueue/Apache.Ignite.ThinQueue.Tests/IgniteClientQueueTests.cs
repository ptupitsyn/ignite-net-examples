using System;
using System.Collections.Generic;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Static;
using NUnit.Framework;

namespace Apache.Ignite.ThinQueue.Tests
{
    public class IgniteClientQueueTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Ignition.Start(GetIgniteConfiguration());
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Ignition.StopAll(true);
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        private static IgniteConfiguration GetIgniteConfiguration() =>
            new()
            {
                DiscoverySpi = new TcpDiscoverySpi
                {
                    IpFinder = new TcpDiscoveryStaticIpFinder
                    {
                        Endpoints = new[] { "127.0.0.1:47500" }
                    },
                    SocketTimeout = TimeSpan.FromSeconds(0.3)
                },
                Localhost = "127.0.0.1",
                JvmOptions = new List<string>
                {
                    "-XX:+HeapDumpOnOutOfMemoryError",
                    "-Xms512m",
                    "-Xmx2g",
                    "-ea",
                    "-DIGNITE_QUIET=true",
                    "-Duser.timezone=UTC"
                }
            };
    }
}
