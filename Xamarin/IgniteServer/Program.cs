using System;
using System.Threading;
using Apache.Ignite.Core;

namespace IgniteServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var ignite = Ignition.Start();

            var people = ignite.GetOrCreateCache<int, Person>("people");
            people[1] = new Person
            {
                Name = "Vasya",
                Address = "USA"
            };
            people[2] = new Person
            {
                Name = "John",
                Address = "India"
            };

            var cars = ignite.GetOrCreateCache<string, Car>("cars");
            cars["car-1"] = new Car
            {
                Make = "Toyozda",
                Model = "Civilla",
                Power = 85
            };
            cars["car-2"] = new Car
            {
                Make = "Mercewagen",
                Model = "Jetta AMG",
                Power = 500
            };

            Thread.Sleep(-1);
        }
    }

    class Person
    {
        public string Name { get; set; }

        public string Address { get; set; }
    }

    class Car
    {
        public string Make { get; set; }
        
        public string Model { get; set; }

        public int Power { get; set; }
    }
}
