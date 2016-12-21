using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ContextFreeTasks.Internal;
using static System.Threading.Tasks.Task;

namespace ContextFreeTasks
{
    [AsyncMethodBuilder(typeof(AsyncContextFreeTaskMethodBuilder))]
    public struct ContextFreeTask
    {
        private readonly Task _task;
        public Task Task => _task ?? FromResult(default(object));
        internal ContextFreeTask(Task t) => _task = t;
        public ContextFreeTaskAwaiter GetAwaiter() => new ContextFreeTaskAwaiter(Task);
        public void Wait() => _task?.Wait();
        public ConfiguredTaskAwaitable ConfigureAwait(bool continueOnCapturedContext) => Task.ConfigureAwait(continueOnCapturedContext);

        public static ContextFreeTask Run(Action action) => new ContextFreeTask(Task.Run(action));
        public static ContextFreeTask Run(Action action, CancellationToken cancellationToken) => new ContextFreeTask(Task.Run(action, cancellationToken));
        public static ContextFreeTask Run(Func<ContextFreeTask> function) => new ContextFreeTask(Task.Run(async () => await function().ConfigureAwait(false)));
        public static ContextFreeTask Run(Func<ContextFreeTask> function, CancellationToken cancellationToken) => new ContextFreeTask(Task.Run(async () => await function().ConfigureAwait(false), cancellationToken));
        public static ContextFreeTask<T> Run<T>(Func<T> function) => new ContextFreeTask<T>(Task.Run(function));
        public static ContextFreeTask<T> Run<T>(Func<T> function, CancellationToken cancellationToken) => new ContextFreeTask<T>(Task.Run(function, cancellationToken));
        public static ContextFreeTask<T> Run<T>(Func<ContextFreeTask<T>> function) => new ContextFreeTask<T>(Task.Run(async () => await function().ConfigureAwait(false)));
        public static ContextFreeTask<T> Run<T>(Func<ContextFreeTask<T>> function, CancellationToken cancellationToken) => new ContextFreeTask<T>(Task.Run(async () => await function().ConfigureAwait(false), cancellationToken));
    }
}
