using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Signal.Tasks.Library;
using Signal.Tasks;
using System.Diagnostics;

namespace SignalTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {

            TaskWorker manager = new TaskWorker();
            //manager.Start();

           // var task = new EchoActivity("ASDFFDSA");

           // manager.AddTaskActivities(task);

            //manager.Stop();

        }
    }


}
