using Apache.Ignite.Core.Client;

namespace Apache.Ignite.ThinQueue
{
    public static class IgniteClientExtensions
    {
        public static IgniteClientQueue<T> GetQueue<T>(this IIgniteClient client, string name) =>
            new IgniteClientQueue<T>(client, name);
    }
}
