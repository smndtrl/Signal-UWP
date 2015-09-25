using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal.Tasks.Library
{
    public class TaskWorker
    {
        volatile bool isStarted;

        TaskDispatcher dispatcher;
        TaskQueue queue;

        public TaskWorker()
        {
            queue = new TaskQueue();
        }

        public TaskWorker Start()
        {
            if (isStarted) throw new Exception("already running");

            dispatcher = new TaskDispatcher(queue);

            dispatcher.Start();

            isStarted = true;
            return this;
        }

        public void Stop()
        {
            dispatcher.Stop();

            isStarted = false;
        }

        public TaskWorker AddTaskActivities(params UntypedTaskActivity[] taskActivityObjects)
        {
            foreach (UntypedTaskActivity instance in taskActivityObjects)
            {
                queue.Add(instance);
                instance.onAdded();
            }

            queue.CompleteAdding();

            return this;
        }
    }
}
