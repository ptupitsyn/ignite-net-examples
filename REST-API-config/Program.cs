using System;
using System.IO;
using System.Net;
using Apache.Ignite.Core;

namespace IgniteNetRestApi
{
    class Program
    {
        static void Main(string[] args)
        {
            // NOTE: This example requires full Ignite 2.8 binary distro to be downloaded and unpacked somewhere: 
            // https://ignite.apache.org/download.cgi

            // TODO: Change this path accordingly
            var restLibsPath = "/home/pavel/Downloads/apache-ignite-2.8.0-bin/libs/optional/ignite-rest-http/";
            var jarFiles = Directory.GetFiles(restLibsPath, "*.jar");
            var classPath = string.Join(":", jarFiles);
            
            var cfg = new IgniteConfiguration
            {
                // Set path to ignite-rest-http module.
                JvmClasspath = classPath,
                
                // This is optional if we want to tweak REST API configuration (e.g. change the port).
                // Otherwise, Spring config is not required.
                SpringConfigUrl = "ignite-spring-config.xml",
            };

            using (var ignite = Ignition.Start(cfg))
            {
                // Check that REST API works. 8080 is the default port.
                var restResponse = new WebClient().DownloadString("http://localhost:8080/ignite?cmd=version");

                Console.WriteLine($">>> Reply from Ignite REST API: {restResponse}");
            }
        }
    }
}