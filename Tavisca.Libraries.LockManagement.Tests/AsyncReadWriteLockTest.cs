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
            var numberList = new List<int>();
            var waitHandle = new CountdownEvent(3);
            var asyncReadWriteLock = new AsyncReadWriteLock();
            //Task.Factory.StartNew(() => getReadLockAction(numberList, waitHandle, asyncReadWriteLock));
            Parallel.Invoke(async () => await getReadLockAction(numberList, waitHandle, asyncReadWriteLock), 
                async () => await getReadLockAction(numberList, waitHandle, asyncReadWriteLock), 
                async () => await getReadLockAction(numberList, waitHandle, asyncReadWriteLock));
            waitHandle.Wait();
            Assert.Equal(3, numberList.Count);
        }
        
        [Fact]
        public void AsyncReadWriteLock_Test_For_Multiple_Write_Lock_Aquired_Sequentially()
        {
            var asyncReadWriteLock = new AsyncReadWriteLock();
            var lockTime = new List<DateTime>();
            var numberList = new List<int>();
            var waitHandle = new CountdownEvent(3);
            Parallel.Invoke(async () => await getWriteLockAction(lockTime, waitHandle, asyncReadWriteLock, numberList),
                async () => await getWriteLockAction(lockTime, waitHandle, asyncReadWriteLock, numberList),
                async () => await getWriteLockAction(lockTime, waitHandle, asyncReadWriteLock, numberList));
            waitHandle.Wait();
            List<DateTime> expectedList = lockTime;
            lockTime.Sort();
            Assert.Equal(expectedList, lockTime);
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
