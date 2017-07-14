import org.apache.ignite.IgniteCheckedException;
import org.apache.ignite.cluster.ClusterNode;
import org.apache.ignite.internal.processors.platform.PlatformPluginExtension;
import org.apache.ignite.plugin.CachePluginContext;
import org.apache.ignite.plugin.CachePluginProvider;
import org.apache.ignite.plugin.ExtensionRegistry;
import org.apache.ignite.plugin.IgnitePlugin;
import org.apache.ignite.plugin.PluginContext;
import org.apache.ignite.plugin.PluginProvider;
import org.apache.ignite.plugin.PluginValidationException;
import org.jetbrains.annotations.Nullable;

import java.io.Serializable;
import java.util.UUID;

public class IgniteNetSemaphorePluginProvider implements PluginProvider<IgniteNetSemaphorePluginConfiguration> {
    public String name() {
        return "DotNetSemaphore";
    }

    public String version() {
        return "1.0";
    }

    public void initExtensions(PluginContext pluginContext, ExtensionRegistry extensionRegistry)
            throws IgniteCheckedException {
        extensionRegistry.registerExtension(PlatformPluginExtension.class,
                new IgniteNetSemaphorePluginExtension(pluginContext.grid()));
    }

    public String copyright() {
        return "-";
    }

    public <T extends IgnitePlugin> T plugin() {
        return (T) new IgniteNetSemaphorePlugin();
    }

    @Nullable
    public <T> T createComponent(PluginContext pluginContext, Class<T> aClass) {
        return null;
    }

    public CachePluginProvider createCacheProvider(CachePluginContext cachePluginContext) {
        return null;
    }

    public void start(PluginContext pluginContext) throws IgniteCheckedException {

    }

    public void stop(boolean b) throws IgniteCheckedException {

    }

    public void onIgniteStart() throws IgniteCheckedException {

    }

    public void onIgniteStop(boolean b) {

    }

    @Nullable
    public Serializable provideDiscoveryData(UUID uuid) {
        return null;
    }

    public void receiveDiscoveryData(UUID uuid, Serializable serializable) {

    }

    public void validateNewNode(ClusterNode clusterNode) throws PluginValidationException {

    }
}
