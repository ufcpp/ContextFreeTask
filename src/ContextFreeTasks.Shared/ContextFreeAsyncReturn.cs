using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ContextFreeTasks.Internal;
using static System.Threading.Tasks.Task;

namespace ContextFreeTasks
{
    [AsyncMethodBuilder(typeof(AsyncContextFreeAsyncReturnMethodBuilder))]
    public struct ContextFreeAsyncReturn
    {
        private readonly Task _task;
        public Task Task => _task ?? FromResult(default(object));
        internal ContextFreeAsyncReturn(Task t) => _task = t;
        public TaskAwaiter GetAwaiter() => Task.GetAwaiter();
        public void Wait() => _task?.Wait();
        public ConfiguredTaskAwaitable ConfigureAwait(bool continueOnCapturedContext) => Task.ConfigureAwait(continueOnCapturedContext);
        public static implicit operator ContextFreeAsyncReturn(Task t) => new ContextFreeAsyncReturn(t);
    }
}
