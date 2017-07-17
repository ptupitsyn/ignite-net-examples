import org.apache.ignite.Ignite;
import org.apache.ignite.internal.processors.platform.PlatformPluginExtension;
import org.apache.ignite.internal.processors.platform.PlatformTarget;

public class IgniteNetSemaphorePluginExtension implements PlatformPluginExtension {
    private final Ignite ignite;

    public IgniteNetSemaphorePluginExtension(Ignite grid) {
        ignite = grid;
    }

    public int id() {
        return 100;
    }

    public PlatformTarget createTarget() {
        return new IgniteNetPluginTarget(ignite);
    }
}
