using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        private const string QueueName = "my-queue";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Ignition.Start(GetIgniteConfiguration());
            Ignition.Start(GetIgniteConfiguration());
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Ignition.StopAll(true);
        }

        [TearDown]
        public void TearDown()
        {
            using var client = StartClient();

            client.GetQueue<int>(QueueName).Close();
        }

        [Test]
        public void TestEnqueueDequeue()
        {
            using var client1 = StartClient();
            using var client2 = StartClient();

            var queue1 = client1.GetQueue<int>("my-queue");
            var queue2 = client1.GetQueue<int>("my-queue");

            Assert.AreEqual(0, queue1.Count);
            Assert.IsFalse(queue1.TryDequeue(out _));

            queue1.Enqueue(1);
            queue2.Enqueue(2);

            Assert.AreEqual(2, queue1.Count);
            Assert.AreEqual(2, queue2.Count);

            Assert.AreEqual(2, queue1.Dequeue());
            Assert.AreEqual(1, queue2.TryDequeue(out var r) ? r : -1);

            var ex = Assert.Throws<IgniteException>(() => queue1.Dequeue());
            Assert.AreEqual("Queue is empty", ex!.Message);
        }

        [Test]
        public void TestBlockingTakeProducerConsumer()
        {
            using var client1 = StartClient();
            using var client2 = StartClient();

            var queue1 = client1.GetQueue<int>("my-queue");
            var queue2 = client1.GetQueue<int>("my-queue");

            Task.Run(() =>
            {
                Thread.Sleep(200);
                queue1.Enqueue(1);

                Thread.Sleep(200);
                queue1.Enqueue(2);
            });

            Assert.IsFalse(queue2.TryDequeue(out _));

            Assert.AreEqual(1, queue2.Take());
            Assert.AreEqual(2, queue2.Take());
        }

        [Test]
        public void TestMultithreaded()
        {
            using var client1 = StartClient();
            using var client2 = StartClient();

            var queue1 = client1.GetQueue<int>("my-queue");
            var queue2 = client1.GetQueue<int>("my-queue");

            var rnd = new Random();
            var taken = new ConcurrentBag<int>();
            var added = new ConcurrentBag<int>();

            Parallel.For(1, 30000, x =>
            {
                var q = x % 2 == 0 ? queue1 : queue2;

                if (rnd.Next() % 2 == 0 && q.Count < 10)
                {
                    q.Enqueue(x);
                    added.Add(x);
                }
                else
                {
                    if (q.TryDequeue(out var r))
                        taken.Add(r);
                }
            });

            while (queue1.TryDequeue(out var r))
                taken.Add(r);

            CollectionAssert.AreEquivalent(added, taken);
        }

        private static IIgniteClient StartClient() =>
            Ignition.StartClient(new IgniteClientConfiguration("127.0.0.1:10800")
            {
                EnablePartitionAwareness = true
            });

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
                },
                AutoGenerateIgniteInstanceName = true
            };
    }
}
