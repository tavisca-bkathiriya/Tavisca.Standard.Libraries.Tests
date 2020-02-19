using System;
using System.Threading;
using Tavisca.Platform.Common;
using Tavisca.Platform.Common.Internal;

namespace Tavisca.Tasks.Core.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //var waitHandle = new CountdownEvent(5);
            //Console.WriteLine("Default Pool Size: " + Environment.ProcessorCount);
            //TaskPool taskPool = new TaskPool(2);
            //RoundRobinPool roundRobinPool = new RoundRobinPool(3);
            //AsyncTasks.UseRoundRobinPool();
            //AsyncTasks.AddPool("TestPool", 2);

            //Action<string> action1 = (x) =>
            //{
            //    Thread.Sleep(3000);
            //    Console.WriteLine(" Action: " + x + " => Thread Id: " + Thread.CurrentThread.ManagedThreadId);
            //    waitHandle.Signal();
            //};

            //Action<string> action2 = (x) =>
            //{
            //    AsyncTasks.AddPool("TestPool", 2);
            //    Console.WriteLine(" Action: " + x + " => Thread Id: " + Thread.CurrentThread.ManagedThreadId);
            //    waitHandle.Signal();
            //};



            //TaskPool Tests
            //taskPool.Enqueue(() => action1("1"));
            //taskPool.Enqueue(() => action2("2"));
            //taskPool.Enqueue(() => action2("3"));
            //taskPool.Enqueue(() => action2("4"));
            //taskPool.Enqueue(() => action2("5"));
            //taskPool.StopAdding();
            //taskPool.Enqueue(() => action2("6"));

            //RoundRobin Tests
            //roundRobinPool.Enqueue(() => action1("1"));
            //roundRobinPool.Enqueue(() => action2("2"));
            //roundRobinPool.Enqueue(() => action2("3"));
            //roundRobinPool.Enqueue(() => action2("4"));
            //roundRobinPool.Enqueue(() => action2("5"));
            //roundRobinPool.StopAdding();
            //roundRobinPool.Enqueue(() => action2("6"));

            //AsyncTask Tests
            //AsyncTasks.Run(() => { action1("1"); }, "testpool");
            //AsyncTasks.Run(() => { action2("2"); }, "testpool");
            //AsyncTasks.Run(() => { action2("3"); }, "testpool");
            //AsyncTasks.Run(() => { action2("4"); }, "testpool");
            //AsyncTasks.Run(() => { action2("5"); }, "testpool");

            //waitHandle.Wait();
            //Console.ReadLine();
        }
    }
}
