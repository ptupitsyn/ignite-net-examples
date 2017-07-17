using Apache.Ignite.Core.Binary;
using Apache.Ignite.Core.Plugin;

namespace IgnitePlugin
{
    [PluginProviderType(typeof(SemaphorePluginProvider))]
    internal class SemaphorePluginConfiguration : IPluginConfiguration
    {
        public void WriteBinary(IBinaryRawWriter writer)
        {
            // No-op.
        }

        public int? PluginConfigurationClosureFactoryId => null;
    }
}