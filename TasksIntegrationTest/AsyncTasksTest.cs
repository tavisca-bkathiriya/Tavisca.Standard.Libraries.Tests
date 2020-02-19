using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;
using Tavisca.Platform.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Tavisca.Libraries.Tasks.Tests
{
    public class AsyncTasksTest
    {
        [Fact]
        public void AsyncTask_Should_Run_Actions_In_Created_Pool()
        {
            var threadIds = new List<int>();
            var waitHandle = new CountdownEvent(5);
            var lockObject = new object();

            AsyncTasks.AddPool("testPool");
            AsyncTasks.Run(() => getTask(lockObject, waitHandle, threadIds));
            AsyncTasks.Run(() => getTask(lockObject, waitHandle, threadIds));
            AsyncTasks.Run(() => getTask(lockObject, waitHandle, threadIds));
            AsyncTasks.Run(() => getTask(lockObject, waitHandle, threadIds));
            AsyncTasks.Run(() => getTask(lockObject, waitHandle, threadIds));
            waitHandle.Wait();

            Assert.Equal(4, threadIds.Distinct().Count());
        }

        private void getTask(Object lockObject, CountdownEvent waitHandle, List<int> threadIds)
        {
            lock (lockObject)
            {
                threadIds.Add(Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(1000);
            }
            waitHandle.Signal();
        }
    }
}
