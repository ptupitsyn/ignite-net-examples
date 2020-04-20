using System;
using Apache.Ignite.Core;

namespace CacheNodeFilter
{
    class Program
    {
        static void Main(string[] args)
        {
            Ignition.Start("ignite-spring-config.xml");
        }
    }
}
