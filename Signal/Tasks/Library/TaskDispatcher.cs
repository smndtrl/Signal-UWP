using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal.Tasks.Library
{
    public sealed class TaskDispatcher
    {
        readonly TaskQueue queue;

        public TaskDispatcher(TaskQueue queue)
        {
            this.queue = queue;
        }

        private bool isStarted = false;

        public void Start()
        {
            Task.Factory.StartNew(() => Dispatch());
        }

        public void Stop()
        {

        }

        private async void Dispatch()
        {
            while(!queue.IsCompleted)
            {
                TaskActivity task = null;

                try
                {
                    //queue.TryTake(out task);
                    
                    task = queue.Take(); // TODO: InvalidOp

                } catch (InvalidOperationException) { }
                

                if (task != null)
                {
                    Task.Factory.StartNew<Task>(Process, task);
                }
            }
            
        }

        private async Task Process(object item)
        {

            TaskActivity task = (TaskActivity)item;
            try
            {
                string output = await task.Run(null, null);
                //eventToRespond = new TaskCompletedEvent(-1, scheduledEvent.EventId, output);
            }
            /*catch (TaskFailureException e)
            {
                TraceHelper.TraceExceptionInstance(TraceEventType.Error, taskMessage.OrchestrationInstance, e);
                string details = IncludeDetails ? e.Details : null;
                eventToRespond = new TaskFailedEvent(-1, scheduledEvent.EventId, e.Message, details);
            }*/
            catch (Exception e)
            {
                /*TraceHelper.TraceExceptionInstance(TraceEventType.Error, taskMessage.OrchestrationInstance, e);
                string details = IncludeDetails
                    ? string.Format("Unhandled exception while executing task: {0}\n\t{1}", e, e.StackTrace)
                    : null;
                eventToRespond = new TaskFailedEvent(-1, scheduledEvent.EventId, e.Message, details);*/
            }


        }
    }

   




}
