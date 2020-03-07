﻿using System;
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
        private Func<AsyncReadWriteLock, CountdownEvent, Task<DateTime>> asyncReadLockAction =
           async (asyncLock, waitHandle) =>
           {
               using (await asyncLock.ReadLockAsync())
               {
                   Thread.Sleep(2000);
                   waitHandle.Signal();
                   return DateTime.Now;
               }
           };

        private Func<AsyncReadWriteLock, CountdownEvent, Task<DateTime>> asyncWriteLockAction =
        async (asyncLock, waitHandle) =>
        {
            using (await asyncLock.WriteLockAsync())
            {
                Thread.Sleep(2000);
                waitHandle.Signal();
                return DateTime.Now;
            }
        };

        //Should be able to acquire read lock for any code block invoked in an async way
        //Calling dispose on async read lock, should release the lock
        //Multiple read operations can be executed in parallel by acquiring read lock

        [Fact]
        public void AsyncReadWriteLock_Test_For_Multiple_Read_Lock_Aquired_At_Same_Time()
        {
            AsyncReadWriteLock asyncLock = new AsyncReadWriteLock();
            CountdownEvent waitHandle = new CountdownEvent(3);           

            DateTime threadTime1 = DateTime.Now, threadTime2 = DateTime.Now, threadTime3 = DateTime.Now;
            Parallel.Invoke(async () => { threadTime1 = await asyncReadLockAction(asyncLock, waitHandle); }, 
                async () => { threadTime2 = await asyncReadLockAction(asyncLock, waitHandle); }, 
                async () => { threadTime3 = await asyncReadLockAction(asyncLock, waitHandle); });
            waitHandle.Wait();
            var timeDiff = (threadTime2 - threadTime1).TotalMilliseconds;
            var timeDiff1 = (threadTime3 - threadTime1).TotalMilliseconds;
            Assert.InRange(Math.Abs(timeDiff), 0, 100);
            Assert.InRange(Math.Abs(timeDiff1), 0, 100);
        }

        //Should be able to acquire write lock for any code block invoked in an async way
        //Calling dispose on async write lock, should release the lock
        [Fact]
        public void AsyncReadWriteLock_Test_For_Multiple_Write_Lock_Aquired_Sequentially()
        {
            AsyncReadWriteLock asyncLock = new AsyncReadWriteLock();
            CountdownEvent waitHandle = new CountdownEvent(2);
            DateTime threadTime1 = DateTime.Now, threadTime2 = DateTime.Now;

            Parallel.Invoke(async () => { threadTime1 = await asyncWriteLockAction(asyncLock, waitHandle); }, 
                async () => { threadTime2 = await asyncWriteLockAction(asyncLock, waitHandle); });
            waitHandle.Wait();
            var timeDiff = (threadTime2 - threadTime1).TotalMilliseconds;
            Assert.InRange(Math.Abs(timeDiff), 2000, 2500);
        }

        //If the object acquire write lock, all the read/write operations should wait for its completion.
        //If multiple read/write operations are waiting in a queue, preference should be given to write operation

        [Fact]
        public async Task AsyncReadWriteLock_Test_For_Write_Lock_Gets_Priority_When_Both_Lock_Are_In_Wait()
        {
            AsyncReadWriteLock asyncLock = new AsyncReadWriteLock();
            CountdownEvent waitHandle = new CountdownEvent(3);
            Task<DateTime> threadTime1Task = null, threadTime2Task = null, threadTime3Task = null;
            
            //First acquire write lock then put read & write lock in wait (first read, then write lock)
            Parallel.Invoke(
                () =>
                {
                    threadTime1Task = asyncWriteLockAction(asyncLock, waitHandle);
                },
                () =>
                {
                    Thread.Sleep(200);
                    threadTime2Task = asyncReadLockAction(asyncLock, waitHandle);
                },
                () =>
                {
                    Thread.Sleep(500);
                    threadTime3Task = asyncWriteLockAction(asyncLock, waitHandle);
                });

            var threadTime1 = threadTime1Task.Result;
            var threadTime2 = threadTime2Task.Result;
            var threadTime3 = threadTime3Task.Result;
            waitHandle.Wait();
            var timeDiffWriteLock = (threadTime3 - threadTime1).TotalMilliseconds;
            var timeDiffReadLock = (threadTime2 - threadTime1).TotalMilliseconds;
            Assert.InRange(timeDiffWriteLock, 2000, 2500);
            Assert.InRange(timeDiffReadLock, 4000, 4500);
        }

        //If the object acquire read lock, all the write operations should wait for its completion.
        //If multiple write operations are waiting in a queue, they will be executed in sequence

        [Fact]
        public async Task AsyncReadWriteLock_Test_For_WriteLocks_Should_Wait_In_Queue_When_ReadLock_Is_Acquired()
        {
            AsyncReadWriteLock asyncLock = new AsyncReadWriteLock();
            CountdownEvent waitHandle = new CountdownEvent(3);
            Task<DateTime> threadTime1Task = null, threadTime2Task = null, threadTime3Task = null;

            //First acquire read lock then put two writes lock in wait (first threadTime3Task, then threadTime2Task)
            Parallel.Invoke(
                () =>
                {
                    threadTime1Task = asyncReadLockAction(asyncLock, waitHandle);
                },
                () =>
                {
                    Thread.Sleep(500);
                    threadTime2Task = asyncWriteLockAction(asyncLock, waitHandle);
                },
                () =>
                {
                    Thread.Sleep(200);
                    threadTime3Task = asyncWriteLockAction(asyncLock, waitHandle);
                });

            var threadTime1 = threadTime1Task.Result;
            var threadTime2 = threadTime2Task.Result;
            var threadTime3 = threadTime3Task.Result;
            waitHandle.Wait();
            var timeDiff1 = (threadTime3 - threadTime1).TotalMilliseconds;
            var timeDiff2 = (threadTime2 - threadTime1).TotalMilliseconds;
            var timeDiff3 = Math.Abs((threadTime2 - threadTime3).TotalMilliseconds);
            Assert.InRange(timeDiff1, 2000, 2500);
            Assert.InRange(timeDiff2, 4000, 4500);
            Assert.InRange(timeDiff3, 2000, 2500);
        }       
    }
}
