using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace ContextFreeTasks.Internal
{
    [StructLayout(LayoutKind.Auto)]
    public struct AsyncContextFreeAsyncReturnMethodBuilder
    {
        private AsyncTaskMethodBuilder _methodBuilder;
        public static AsyncContextFreeAsyncReturnMethodBuilder Create() =>
            new AsyncContextFreeAsyncReturnMethodBuilder() { _methodBuilder = AsyncTaskMethodBuilder.Create() };
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine => _methodBuilder.Start(ref stateMachine);
        public void SetStateMachine(IAsyncStateMachine stateMachine) => _methodBuilder.SetStateMachine(stateMachine);
        public void SetResult() => _methodBuilder.SetResult();
        public void SetException(Exception exception) => _methodBuilder.SetException(exception);
        public ContextFreeAsyncReturn Task => new ContextFreeAsyncReturn(_methodBuilder.Task);
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
