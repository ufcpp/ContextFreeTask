using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static System.Threading.Tasks.Task;

namespace ContextFreeTasks
{
    [AsyncMethodBuilder(typeof(AsyncContextFreeTaskMethodBuilder<>))]
    public struct ContextFreeTask<T>
    {
        private readonly Task<T> _task;
        public Task<T> Task => _task ?? FromResult(default(T));
        public T Result => GetAwaiter().GetResult();
        internal ContextFreeTask(Task<T> t) => _task = t;
        public ContextFreeTaskAwaiter<T> GetAwaiter() => new ContextFreeTaskAwaiter<T>(Task);
        public void Wait() => _task?.Wait();
        public ConfiguredTaskAwaitable<T> ConfigureAwait(bool continueOnCapturedContext) => Task.ConfigureAwait(continueOnCapturedContext);
    }
}
