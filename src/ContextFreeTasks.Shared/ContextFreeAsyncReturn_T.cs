using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ContextFreeTasks.Internal;
using static System.Threading.Tasks.Task;

namespace ContextFreeTasks
{
    [AsyncMethodBuilder(typeof(AsyncContextFreeAsyncReturnMethodBuilder<>))]
    public struct ContextFreeAsyncReturn<T>
    {
        private readonly Task<T> _task;
        public Task<T> Task => _task ?? FromResult(default(T));
        public T Result => GetAwaiter().GetResult();
        internal ContextFreeAsyncReturn(Task<T> t) => _task = t;
        public TaskAwaiter<T> GetAwaiter() => Task.GetAwaiter();
        public void Wait() => _task?.Wait();
        public ConfiguredTaskAwaitable<T> ConfigureAwait(bool continueOnCapturedContext) => Task.ConfigureAwait(continueOnCapturedContext);
    }
}
