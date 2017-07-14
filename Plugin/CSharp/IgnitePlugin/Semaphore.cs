using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apache.Ignite.Core.Interop;

namespace IgnitePlugin
{
    public class Semaphore
    {
        private readonly IPlatformTarget _target;

        public Semaphore(IPlatformTarget target)
        {
            _target = target;
        }

        public void WaitOne()
        {
            _target.InLongOutLong(0, 0);
        }

        public void Release()
        {
            _target.InLongOutLong(1, 0);
        }
    }
}
