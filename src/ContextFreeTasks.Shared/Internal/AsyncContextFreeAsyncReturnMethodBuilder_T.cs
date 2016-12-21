using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace ContextFreeTasks.Internal
{
    [StructLayout(LayoutKind.Auto)]
    public struct AsyncContextFreeAsyncReturnMethodBuilder<T>
    {
        private AsyncTaskMethodBuilder<T> _methodBuilder;
        public static AsyncContextFreeAsyncReturnMethodBuilder<T> Create() =>
            new AsyncContextFreeAsyncReturnMethodBuilder<T>() { _methodBuilder = AsyncTaskMethodBuilder<T>.Create() };
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine => _methodBuilder.Start(ref stateMachine);
        public void SetStateMachine(IAsyncStateMachine stateMachine) => _methodBuilder.SetStateMachine(stateMachine);
        public void SetResult(T result) => _methodBuilder.SetResult(result);
        public void SetException(Exception exception) => _methodBuilder.SetException(exception);
        public ContextFreeAsyncReturn<T> Task => new ContextFreeAsyncReturn<T>(_methodBuilder.Task);
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            SynchronizationContext.SetSynchronizationContext(null);
            _methodBuilder.AwaitOnCompleted(ref awaiter, ref stateMachine);
        }
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
            SynchronizationContext.SetSynchronizationContext(null);
            _methodBuilder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
        }
    }
}
