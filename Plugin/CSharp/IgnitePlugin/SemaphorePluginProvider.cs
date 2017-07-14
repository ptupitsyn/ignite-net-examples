using Apache.Ignite.Core.Plugin;

namespace IgnitePlugin
{
    class SemaphorePluginProvider : IPluginProvider<SemaphorePluginConfiguration>
    {
        private SemaphorePlugin _plugin;

        public T GetPlugin<T>() where T : class
        {
            return _plugin as T;
        }

        public void Start(IPluginContext<SemaphorePluginConfiguration> context)
        {
            _plugin = new SemaphorePlugin(context);
        }

        public void Stop(bool cancel)
        {
            // No-op.
        }

        public void OnIgniteStart()
        {
            // No-op.
        }

        public void OnIgniteStop(bool cancel)
        {
            // No-op.
        }

        public string Name { get; }
        public string Copyright { get; }
    }
}