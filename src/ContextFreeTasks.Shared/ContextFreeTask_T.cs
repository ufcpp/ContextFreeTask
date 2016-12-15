using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ContextFreeTasks
{
    [AsyncMethodBuilder(typeof(AsyncContextFreeTaskMethodBuilder<>))]
    public struct ContextFreeTask<T>
    {
        public Task<T> Task { get; }
        internal ContextFreeTask(Task<T> t) => Task = t;
        public ContextFreeTaskAwaiter<T> GetAwaiter() => new ContextFreeTaskAwaiter<T>(Task);
    }
}
