using System;
using System.Collections.Concurrent;
using System.Threading;

namespace UnitTest
{
    class SingleThreadedSynchronizationContext : SynchronizationContext, IDisposable
    {
        private struct WorkItem
        {
            private readonly SendOrPostCallback _callback;
            private readonly object _state;

            public WorkItem(SendOrPostCallback callback, object state)
            {
                _callback = callback ?? throw new ArgumentNullException("callback");
                _state = state;
            }

            public void Execute() => _callback(_state);
        }

        private readonly ConcurrentQueue<WorkItem> _workItems = new ConcurrentQueue<WorkItem>();
        private bool _isStopped = false;
        private readonly Thread _thread;

        public SingleThreadedSynchronizationContext()
        {
            _thread = new Thread(Run);
            SetSynchronizationContext(this);
            _thread.Start();
        }

        public int ThreadId => _thread.ManagedThreadId;

        private void Run()
        {
            SetSynchronizationContext(this);
            while (!_isStopped)
            {
                while (!_workItems.IsEmpty)
                {
                    if (_workItems.TryDequeue(out var currentItem))
                    {
                        currentItem.Execute();
                    }
                }
                Thread.Sleep(10);
            }
        }

        public override void Post(SendOrPostCallback d, object state) => _workItems.Enqueue(new WorkItem(d, state));
        public override void Send(SendOrPostCallback d, object state) => throw new NotSupportedException();

        public void Dispose()
        {
            _isStopped = true;
            _thread.Join();
        }
    }
}
