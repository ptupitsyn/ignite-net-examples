using Apache.Ignite.Core.Interop;
using Apache.Ignite.Core.Plugin;

namespace IgnitePlugin
{
    internal class SemaphorePlugin
    {
        private readonly IPlatformTarget _target;

        public SemaphorePlugin(IPluginContext<SemaphorePluginConfiguration> context)
        {
            _target = context.GetExtension(100);
        }

        public Semaphore GetOrCreateSemaphore(string name, int count)
        {
            var semaphoreTarget = _target.InStreamOutObject(0, w =>
            {
                w.WriteString(name);
                w.WriteInt(count);
            });

            return new Semaphore(semaphoreTarget);
        }
    }
}