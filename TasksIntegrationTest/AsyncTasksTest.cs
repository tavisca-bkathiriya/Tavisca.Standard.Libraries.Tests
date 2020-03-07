using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;
using Tavisca.Platform.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Tavisca.Platform.Common.Internal;

namespace Tavisca.Libraries.Tasks.Tests
{
    public class AsyncTasksTest
    {
        // Should be able to configure a default task pool with default size for a set of actions. Default size would be equal to number of processors on the machine.
        // Should be able to enqueue an action for configured task pool
        [Fact]
        public void AsyncTask_Should_Run_Actions_In_Created_Pool()
        {
            var threadIds = new ConcurrentBag<int>();
            var waitHandle = new CountdownEvent(10);

            Action runTask = () => {
                Thread.Sleep(1000);
                threadIds.Add(Thread.CurrentThread.ManagedThreadId);
                waitHandle.Signal();
            };

            AsyncTasks.AddPool("testPool");
            AsyncTasks.Run(runTask, "testPool");
            AsyncTasks.Run(runTask, "testPool");
            AsyncTasks.Run(runTask, "testPool");
            AsyncTasks.Run(runTask, "testPool");
            AsyncTasks.Run(runTask, "testPool");
            AsyncTasks.Run(runTask, "testPool");
            AsyncTasks.Run(runTask, "testPool");
            AsyncTasks.Run(runTask, "testPool");
            AsyncTasks.Run(runTask, "testPool");
            AsyncTasks.Run(runTask, "testPool");
            waitHandle.Wait();
            AsyncTasks.RemoveAll();

            Assert.Equal(Environment.ProcessorCount, threadIds.Distinct().Count());
        }

        //Should be able to configure a default task pool with specific size for a set of actions
        [Fact]
        public void AsyncTask_Should_Run_Actions_In_Created_Pool_With_GivenSize()
        {
            var threadIds = new ConcurrentBag<int>();
            var waitHandle = new CountdownEvent(4);

            Action runTask = () => {
                Thread.Sleep(1000);
                threadIds.Add(Thread.CurrentThread.ManagedThreadId);
                waitHandle.Signal();
            };

            int poolSize = 2;
            AsyncTasks.AddPool("testPool1", poolSize);
            AsyncTasks.Run(runTask, "testPool1");
            AsyncTasks.Run(runTask, "testPool1");
            AsyncTasks.Run(runTask, "testPool1");
            AsyncTasks.Run(runTask, "testPool1");
            waitHandle.Wait();
            AsyncTasks.RemoveAll();

            Assert.Equal(poolSize, threadIds.Distinct().Count());
        }

        // Should be able to configure a default task pool for a set of actions
        [Fact]
        public void AsyncTask_Should_Run_Actions_In_Created_Pool_With_GivenSize2()
        {
            var threadIds = new ConcurrentBag<int>();
            var waitHandle = new CountdownEvent(4);

            Action runTask = () => {
                Thread.Sleep(1000);
                threadIds.Add(Thread.CurrentThread.ManagedThreadId);
                waitHandle.Signal();
            };

            int poolSize = 2;
            var taskPool = new TaskPool(poolSize);
            AsyncTasks.AddPool("testPool2", taskPool);
            AsyncTasks.Run(runTask, "testPool2");
            AsyncTasks.Run(runTask, "testPool2");
            AsyncTasks.Run(runTask, "testPool2");
            AsyncTasks.Run(runTask, "testPool2");
            waitHandle.Wait();
            AsyncTasks.RemoveAll();

            Assert.Equal(poolSize, threadIds.Distinct().Count());
        }

        //Should be able to configure a round robin task pool with default size for a set of actions.Default size would be equal to number of processors on the machine.
        [Fact]
        public void AsyncTask_Should_Run_Actions_In_Created_RoundRobinPool()
        {
            var waitHandle = new CountdownEvent(10);

            Func<int> runTask = () => {
                waitHandle.Signal();
                return Thread.CurrentThread.ManagedThreadId;
            };

            int threadId1 = 0, threadId2 = 0, threadId3 = 0, threadId4 = 0, threadId5 = 0;
            int threadId6 = 0, threadId7 = 0, threadId8 = 0, threadId9 = 0, threadId10 = 0;
            AsyncTasks.UseRoundRobinPool();
            AsyncTasks.AddPool("testPool3");
            AsyncTasks.Run(() => { threadId1 = runTask(); }, "testPool3");
            AsyncTasks.Run(() => { threadId2 = runTask(); }, "testPool3");
            AsyncTasks.Run(() => { threadId3 = runTask(); }, "testPool3");
            AsyncTasks.Run(() => { threadId4 = runTask(); }, "testPool3");
            AsyncTasks.Run(() => { threadId5 = runTask(); }, "testPool3");
            AsyncTasks.Run(() => { threadId6 = runTask(); }, "testPool3");
            AsyncTasks.Run(() => { threadId7 = runTask(); }, "testPool3");
            AsyncTasks.Run(() => { threadId8 = runTask(); }, "testPool3");
            AsyncTasks.Run(() => { threadId9 = runTask(); }, "testPool3");
            AsyncTasks.Run(() => { threadId10 = runTask(); }, "testPool3");
            waitHandle.Wait();
            AsyncTasks.RemoveAll();

            //Works only if Environment.ProcessorCount = 8
            Assert.Equal(threadId1, threadId9);
            Assert.Equal(threadId2, threadId10);
            Assert.NotEqual(threadId1, threadId5);
            Assert.NotEqual(threadId4, threadId8);
        }

        //Should be able to configure a round robin task pool with specific size for a set of actions
        //In case of round robin pool, number of queues should be same as the configured size and actions should be enqueued in round robin manner in each queue

        [Fact]
        public void AsyncTask_Should_Run_Actions_In_Created_RoundRobinPool_With_GivenSize()
        {
            var waitHandle = new CountdownEvent(5);

            Func<int> runTask = () => {
                waitHandle.Signal();
                return Thread.CurrentThread.ManagedThreadId;
            };

            int poolSize = 2;
            int threadId1 = 0, threadId2 = 0, threadId3 = 0, threadId4 =0, threadId5 = 0;
            AsyncTasks.UseRoundRobinPool();
            AsyncTasks.AddPool("testPool4", poolSize);
            AsyncTasks.Run(() => { threadId1 = runTask(); }, "testPool4");
            AsyncTasks.Run(() => { threadId2 = runTask(); }, "testPool4");
            AsyncTasks.Run(() => { threadId3 = runTask(); }, "testPool4");
            AsyncTasks.Run(() => { threadId4 = runTask(); }, "testPool4");
            AsyncTasks.Run(() => { threadId5 = runTask(); }, "testPool4");
            waitHandle.Wait();
            AsyncTasks.RemoveAll();

            Assert.Equal(threadId1, threadId3);
            Assert.Equal(threadId1, threadId5);
            Assert.Equal(threadId2, threadId2);
            Assert.NotEqual(threadId1, threadId2);
        }

        //Should be able to configure a round robin task pool for a set of actions
        [Fact]
        public void AsyncTask_Should_Run_Actions_In_Created_RoundRobinPool_With_GivenSize2()
        {
            var waitHandle = new CountdownEvent(5);

            Func<int> runTask = () => {
                waitHandle.Signal();
                return Thread.CurrentThread.ManagedThreadId;
            };

            int poolSize = 2;
            int threadId1 = 0, threadId2 = 0, threadId3 = 0, threadId4 = 0, threadId5 = 0;
            var pool = new RoundRobinPool(poolSize);
            AsyncTasks.AddPool("testPool5", pool);
            AsyncTasks.Run(() => { threadId1 = runTask(); }, "testPool5");
            AsyncTasks.Run(() => { threadId2 = runTask(); }, "testPool5");
            AsyncTasks.Run(() => { threadId3 = runTask(); }, "testPool5");
            AsyncTasks.Run(() => { threadId4 = runTask(); }, "testPool5");
            AsyncTasks.Run(() => { threadId5 = runTask(); }, "testPool5");
            waitHandle.Wait();
            AsyncTasks.RemoveAll();

            Assert.Equal(threadId1, threadId3);
            Assert.Equal(threadId1, threadId5);
            Assert.Equal(threadId2, threadId2);
            Assert.NotEqual(threadId1, threadId2);
        }
        
        //Should be able to enqueue an action even without configuring a taskpool, it should implicitely configure a taskpool with the supplied name
        [Fact]
        public void Should_Be_Able_To_Enqueue_Actions_Without_Configuring_Taskpool()
        {
            var threadIds = new ConcurrentBag<int>();
            var waitHandle = new CountdownEvent(10);

            Action runTask = () => {
                Thread.Sleep(1000);
                threadIds.Add(Thread.CurrentThread.ManagedThreadId);
                waitHandle.Signal();
            };

            AsyncTasks.Run(runTask, "testPool6");
            AsyncTasks.Run(runTask, "testPool6");
            AsyncTasks.Run(runTask, "testPool6");
            AsyncTasks.Run(runTask, "testPool6");
            AsyncTasks.Run(runTask, "testPool6");
            AsyncTasks.Run(runTask, "testPool6");
            AsyncTasks.Run(runTask, "testPool6");
            AsyncTasks.Run(runTask, "testPool6");
            AsyncTasks.Run(runTask, "testPool6");
            AsyncTasks.Run(runTask, "testPool6");
            waitHandle.Wait();
            AsyncTasks.RemoveAll();

            Assert.Equal(Environment.ProcessorCount, threadIds.Distinct().Count());
        }

        //Should be able to enqueue an action even without configuring a taskpool & supplying a name, it should implicitely configure a taskpool with a default name
        [Fact]
        public void Should_Be_Able_To_Enqueue_Actions_Without_Configuring_Taskpool_And_Name()
        {
            var threadIds = new ConcurrentBag<int>();
            var waitHandle = new CountdownEvent(10);

            Action runTask = () => {
                Thread.Sleep(1000);
                threadIds.Add(Thread.CurrentThread.ManagedThreadId);
                waitHandle.Signal();
            };

            AsyncTasks.Run(runTask);
            AsyncTasks.Run(runTask);
            AsyncTasks.Run(runTask);
            AsyncTasks.Run(runTask);
            AsyncTasks.Run(runTask);
            AsyncTasks.Run(runTask);
            AsyncTasks.Run(runTask);
            AsyncTasks.Run(runTask);
            AsyncTasks.Run(runTask);
            AsyncTasks.Run(runTask);
            waitHandle.Wait();
            AsyncTasks.RemoveAll();

            Assert.Equal(Environment.ProcessorCount, threadIds.Distinct().Count());
        }
        
        //Should be able to enqueue actions/ add pool from multiple threads in a thread safe manner.
        [Fact]
        public void Should_Be_Able_To_Enqueue_Actions_From_Multiple_Threads()
        {
            var threadIds = new ConcurrentBag<int>();
            var waitHandle = new CountdownEvent(4);

            Action runTask = () => {
                Thread.Sleep(1000);
                threadIds.Add(Thread.CurrentThread.ManagedThreadId);
                waitHandle.Signal();
            };


            AsyncTasks.AddPool("testPool7", 2);
            Parallel.Invoke(
                () => {
                    AsyncTasks.Run(runTask, "testPool7");
                },
                () => {
                    AsyncTasks.AddPool("testPool8", 1);
                    AsyncTasks.Run(runTask, "testPool8");
                },
                () => {
                    AsyncTasks.Run(runTask, "testPool7");
                },
                () => {
                    AsyncTasks.AddPool("testPool9", 1);
                    AsyncTasks.Run(runTask, "testPool9");
                }
            );

            waitHandle.Wait();
            AsyncTasks.RemoveAll();

            Assert.Equal(4, threadIds.Distinct().Count());
        }

        //In case of default pool, only one queue is available and actions should be executed in FIFO manner
        [Fact]
        public void AsyncTask_Default_Pool_Should_Execute_Action_In_FIFO_manner()
        {
            var threadIds = new ConcurrentBag<int>();
            var waitHandle = new CountdownEvent(3);

            DateTime dateTime1 = new DateTime(), dateTime2 = new DateTime(), dateTime3 = new DateTime(), dateTime4 = new DateTime(); 
            Action runTask1 = () => {
                Thread.Sleep(1000);
                waitHandle.Signal();
                dateTime1 = DateTime.Now;
            };

            Action runTask2 = () => {
                Thread.Sleep(1000);
                waitHandle.Signal();
                dateTime2 = DateTime.Now;
            };

            Action runTask3 = () => {
                Thread.Sleep(1000);
                waitHandle.Signal();
                dateTime3 = DateTime.Now;
            };
            
            int poolSize = 2;
            AsyncTasks.AddPool("testPool10", poolSize);

            Parallel.Invoke(
               () => {
                   Thread.Sleep(500);
                   AsyncTasks.Run(runTask1, "testPool10");
               },
               () => {
                   AsyncTasks.Run(runTask2, "testPool10");
               },
               () => {
                   Thread.Sleep(200);
                   AsyncTasks.Run(runTask3, "testPool10");
               }
           );

            waitHandle.Wait();
            AsyncTasks.RemoveAll();

            Assert.True(dateTime1 > dateTime2);
            Assert.True(dateTime1 > dateTime3);
            Assert.True(dateTime3 > dateTime2);
        }
    }
}
