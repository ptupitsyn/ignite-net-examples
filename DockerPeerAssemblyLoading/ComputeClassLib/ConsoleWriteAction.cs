using System;
using Apache.Ignite.Core.Compute;

namespace ComputeClassLib
{
    public class ConsoleWriteAction : IComputeAction
    {
        public void Invoke()
        {
            Console.WriteLine(">>> TEST");
        }
    }
}
