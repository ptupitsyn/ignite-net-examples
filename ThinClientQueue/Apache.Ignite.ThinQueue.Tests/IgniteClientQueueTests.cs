using System;
using System.Collections.Generic;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Client;
using Apache.Ignite.Core.Common;
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
        public void TestEnqueueDequeue()
        {
            using var client1 = StartClient();
            using var client2 = StartClient();

            var queue1 = client1.GetQueue<int>("my-queue");
            var queue2 = client1.GetQueue<int>("my-queue");

            queue1.Enqueue(1);
            queue2.Enqueue(2);

            Assert.AreEqual(2, queue1.Dequeue());
            Assert.AreEqual(1, queue2.Dequeue());

            var ex = Assert.Throws<IgniteException>(() => queue1.Dequeue());
            Assert.IsNotNull(ex);
            Assert.AreEqual("Queue is empty", ex.Message);

            queue1.Close();
            queue2.Close();
        }

        private static IIgniteClient StartClient() =>
            Ignition.StartClient(new IgniteClientConfiguration("127.0.0.1:10800"));

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
