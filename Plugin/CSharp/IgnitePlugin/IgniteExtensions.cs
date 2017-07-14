using Apache.Ignite.Core;

namespace IgnitePlugin
{
    public static class IgniteExtensions
    {
        public static Semaphore GetOrCreateSemaphore(this IIgnite ignite, string name, int count)
        {
            return ignite.GetPlugin<SemaphorePlugin>("semaphorePlugin").GetOrCreateSemaphore(name, count);
        }
    }
}
