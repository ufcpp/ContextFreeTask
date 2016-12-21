using System.Threading.Tasks;
using ContextFreeTasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class UnitTestContextFreeAsyncReturn
    {
        SingleThreadedSynchronizationContext _syncContext;

        [TestInitialize]
        public void Initialize()
        {
            _syncContext = new SingleThreadedSynchronizationContext();
        }

        [TestCleanup]
        public void Creanup()
        {
            _syncContext.Dispose();
        }

        [TestMethod]
        public void TestMethod1()
        {
            TestMethod1Async().Wait();
        }

        private async Task TestMethod1Async()
        {
            await Task.Run(() => { });
            OnMainThread();
            await A1(10);
            OnMainThread();
            await B1(10);
            OnMainThread();
        }

        private void OnThread(int expectedThreadId)
        {
            var threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            Assert.AreEqual(expectedThreadId, threadId);
        }

        private void OnMainThread() => OnThread(_syncContext.ThreadId);

        private void OnPoolThread()
        {
            var threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            Assert.AreNotEqual(_syncContext.ThreadId, threadId);
        }

        private async Task<string> A1(int n)
        {
            OnMainThread();
            var s = await A2(n);
            OnMainThread();
            await Task.Delay(100);
            OnMainThread();
            await A3();
            OnMainThread();
            return s;
        }

        private async ContextFreeAsyncReturn<string> A2(int n)
        {
            await Task.Delay(100);
            OnPoolThread();
            await Task.Delay(100);
            OnPoolThread();
            await A3();
            OnPoolThread();
            return n.ToString();
        }

        private async ContextFreeAsyncReturn A3()
        {
            await Task.Delay(100);
            OnPoolThread();
        }

        private async Task<string> B1(int n)
        {
            OnMainThread();
            var s = await B2(n);
            OnMainThread();
            await Task.Delay(100);
            OnMainThread();
            await B3();
            OnMainThread();
            return s;
        }

        private async Task<string> B2(int n)
        {
            await Task.Delay(100);
            OnMainThread();
            await Task.Delay(100);
            OnMainThread();
            await B3();
            OnMainThread();
            return n.ToString();
        }

        private async Task B3()
        {
            await Task.Delay(100);
            OnMainThread();
        }
    }
}
