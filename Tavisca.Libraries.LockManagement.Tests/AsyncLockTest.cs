using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tavisca.Platform.Common;
using Xunit;

namespace Tavisca.Libraries.LockManagement.Tests
{
    public class AsyncLockTest
    {
        private static AsyncLock asyncLock = new AsyncLock();
        private static  List<DateTime> lockTime = new List<DateTime>();
        private static CountdownEvent waitHandle = new CountdownEvent(5);
        Action<int> action = async (x) =>
        {
            using (await asyncLock.LockAsync())
            {
                Thread.Sleep(2000);
                lockTime.Add(DateTime.Now);
                waitHandle.Signal();
            }
        };

        [Fact]
        public void AsyncLock_Should_Give_Serialise_Access_To_Object_In_Asynchronous_Way()
        {
            Parallel.Invoke(() => action(1), () => action(2), () => action(3), () => action(4), () => action(5));
            waitHandle.Wait();
            List<DateTime> expected = lockTime;
            lockTime.Sort();
            Assert.Equal(expected, lockTime);
        }
    }
}
