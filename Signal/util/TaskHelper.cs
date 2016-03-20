

using Signal;
using Signal.Tasks;
/** 
* Copyright (C) 2015 smndtrl
* 
* This program is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
* 
* You should have received a copy of the GNU General Public License
* along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;

namespace Signal.Util
{
    public class TaskHelper
    {
        private const string TASK_ENTRY_POINT = "SignalTasks.PushReceiverTask";
        private const string TASK_NAME = "PushReceiver";

        private static readonly Object instanceLock = new Object();
        private static volatile TaskHelper instance;

        public static TaskHelper getInstance()
        {
            if (instance == null)
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new TaskHelper();
                    }
                }
            }

            return instance;
        }

        private TaskHelper() { }

        public async Task RegisterAll()
        {
            foreach (var task in Tasks)
            {
               await Register(task);
            }
        }

        public async Task Register(TaskInformation info)
        {
            bool found = false;
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == info.name) found = true;
            }

            if (!found)
            {
                var reg =
                    await
                        RegisterBackgroundTask(info.taskEntryPoint, info.name, info.trigger, info.condition,
                            info.background);
            }
        }

        public List<TaskInformation> Tasks = new List<TaskInformation>()
        {
            new TaskInformation()
            {
                taskEntryPoint = "SignalTasks.WebsocketTask",
                name = "WebsocketTask",
                trigger = new TimeTrigger(15, false),
                condition = null, //new SystemCondition(SystemConditionType.InternetAvailable),
                background = true
            }
        };

        public struct TaskInformation
        {
            public string taskEntryPoint;
            public string name;
            public IBackgroundTrigger trigger;
            public IBackgroundCondition condition;
            public bool background;
            public delegate void TaskCompletedCallback(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args);
        }

        public static async Task<BackgroundTaskRegistration> RegisterBackgroundTask(TaskInformation info)
        {
            if (info.background)
            {
                await BackgroundExecutionManager.RequestAccessAsync();
            }

            var builder = new BackgroundTaskBuilder();

            builder.Name = info.name;
            builder.TaskEntryPoint = info.taskEntryPoint;
            builder.SetTrigger(info.trigger);

            if (info.condition != null)
            {
                builder.AddCondition(info.condition);

                builder.CancelOnConditionLoss = true;
            }


            BackgroundTaskRegistration task = builder.Register();

            /*if (info.callback != null)
            {
                task.Completed +=
            }*/

            return task;
        }

        private static void Aasda(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            throw new NotImplementedException();
        }

        public static void UnregisterBackgroundTasks(string name)
        {
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == name) cur.Value.Unregister(true);
            }
        }

        /*public void RegisterPushReceiver()
        {
            RegisterBackgroundTask();
        }

        private void RegisterBackgroundTask()
        {
            BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
            PushNotificationTrigger trigger = new PushNotificationTrigger();
            taskBuilder.SetTrigger(trigger);

            // Background tasks must live in separate DLL, and be included in the package manifest
            // Also, make sure that your main application project includes a reference to this DLL
            taskBuilder.TaskEntryPoint = TASK_ENTRY_POINT;
            taskBuilder.Name = TASK_NAME;

            try
            {
                BackgroundTaskRegistration task = taskBuilder.Register();
                Debug.WriteLine("task registered");
                task.Completed += BackgroundTaskCompleted;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error {ex.Message}");
                //rootPage.NotifyUser("Registration error: " + ex.Message, NotifyType.ErrorMessage);
                UnregisterBackgroundTask();
            }
        }

        private void BackgroundTaskCompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            var settings = ApplicationData.Current.LocalSettings;
            var key = sender.TaskId.ToString();
            var message = settings.Values[key].ToString();
            var split = message.Split(new char[] { ';' });

            if (split[0].Equals("message"))
            {
                App.Current.Worker.AddTaskActivities(new PushContentReceiveTask(split[1]));

            }

            throw new NotImplementedException("TaskHelper BackgroundTaskComplete");
        }

        private bool UnregisterBackgroundTask()
        {
            foreach (var iter in BackgroundTaskRegistration.AllTasks)
            {
                IBackgroundTaskRegistration task = iter.Value;
                if (task.Name == TASK_NAME)
                {
                    task.Unregister(true);
                    return true;
                }
            }
            return false;
        }*/
    }
}
