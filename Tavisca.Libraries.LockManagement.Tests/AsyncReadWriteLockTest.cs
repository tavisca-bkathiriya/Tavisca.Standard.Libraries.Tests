using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tavisca.Platform.Common;
using Xunit;

namespace Tavisca.Libraries.LockManagement.Tests
{
    public class AsyncReadWriteLockTest
    {
        [Fact]
        public void AsyncReadWriteLock_Test_For_Multiple_Read_Lock_Aquired_At_Same_Time()
        {
            AsyncReadWriteLock asyncLock = new AsyncReadWriteLock();
            CountdownEvent waitHandle = new CountdownEvent(3);
            Func<Task<DateTime>> asyncReadLockAction = async () =>
            {
                using (await asyncLock.ReadLockAsync())
                {
                    Thread.Sleep(2000);
                    waitHandle.Signal();
                    return DateTime.Now;
                }
            };
            DateTime threadTime1 = DateTime.Now, threadTime2 = DateTime.Now, threadTime3 = DateTime.Now;
            Parallel.Invoke(async () => { threadTime1 = await asyncReadLockAction(); }, 
                async () => { threadTime2 = await asyncReadLockAction(); }, 
                async () => { threadTime3 = await asyncReadLockAction(); });
            waitHandle.Wait();
            var timeDiff = (threadTime2 - threadTime1).TotalMilliseconds;
            var timeDiff1 = (threadTime3 - threadTime1).TotalMilliseconds;
            Assert.InRange(Math.Abs(timeDiff), 0, 100);
            Assert.InRange(Math.Abs(timeDiff1), 0, 100);
        }
        
        [Fact]
        public void AsyncReadWriteLock_Test_For_Multiple_Write_Lock_Aquired_Sequentially()
        {
            AsyncReadWriteLock asyncLock = new AsyncReadWriteLock();
            CountdownEvent waitHandle = new CountdownEvent(2);
            Func<Task<DateTime>> asyncLockAction = async () =>
            {
                using (await asyncLock.WriteLockAsync())
                {
                    Thread.Sleep(2000);
                    waitHandle.Signal();
                    return DateTime.Now;
                }
            };
            DateTime threadTime1 = DateTime.Now, threadTime2 = DateTime.Now;
            Parallel.Invoke(async () => { threadTime1 = await asyncLockAction(); }, async () => { threadTime2 = await asyncLockAction(); });
            waitHandle.Wait();
            var timeDiff = (threadTime2 - threadTime1).TotalMilliseconds;
            Assert.InRange(Math.Abs(timeDiff), 2000, 4000);
        }

        [Fact]
        public async Task AsyncReadWriteLock_Test_For_Write_Lock_Gets_Priority_When_Both_Lock_Are_In_Wait()
        {
            var lockTime = new List<DateTime>();
            var numberList = new List<int>();
            var waitHandle = new CountdownEvent(3);
            var asyncReadWriteLock = new AsyncReadWriteLock();
            var writeLock = await asyncReadWriteLock.WriteLockAsync();
            Parallel.Invoke(async () => await getReadLockAction(numberList, waitHandle, asyncReadWriteLock),
                async () => await getWriteLockAction(lockTime, waitHandle, asyncReadWriteLock, numberList),
                async () => await getReadLockAction(numberList, waitHandle, asyncReadWriteLock));
            writeLock.Dispose();
            waitHandle.Wait();
            Assert.Equal(0, numberList.IndexOf(2));
        }

        [Fact]
        public async Task AsyncReadWriteLock_Test_For_Write_Lock_Wait_When_ReadLock_Aquired()
        {
            var numberList = new List<int>();
            var asyncReadWriteLock = new AsyncReadWriteLock();
            var readLock = await asyncReadWriteLock.ReadLockAsync();
            var writeTask = Task.Run(async () => 
            { 
                using (await asyncReadWriteLock.WriteLockAsync())
                {
                    numberList.Add(2);
                }
            });
            numberList.Add(1);
            readLock.Dispose();
            await writeTask;
            Assert.Equal(1, numberList.IndexOf(2));
        }

        private async Task getReadLockAction(List<int> numberList, CountdownEvent waitHandle, AsyncReadWriteLock asyncReadWriteLock)
        {
                using (await asyncReadWriteLock.ReadLockAsync())
                {
                    numberList.Add(1);
                    waitHandle.Signal();
                }
        }
        
        private async Task getWriteLockAction(List<DateTime> lockTime, CountdownEvent waitHandle, AsyncReadWriteLock asyncReadWriteLock, List<int> numberList)
        {
            using (await asyncReadWriteLock.WriteLockAsync())
                {
                    lockTime.Add(DateTime.Now);
                numberList.Add(2);
                    Thread.Sleep(2000);
                    waitHandle.Signal();
                }
        }
    }
}
