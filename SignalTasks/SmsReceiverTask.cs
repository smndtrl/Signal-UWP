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
using Windows.Devices.Sms;
using Windows.Networking.PushNotifications;
using Windows.Storage;

namespace SignalTasks
{

    public sealed class SmsReceiverTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
            string taskName = taskInstance.Task.Name;

            var smsDetails = taskInstance.TriggerDetails as SmsMessageReceivedTriggerDetails;

            SmsTextMessage2 smsTextMessage = smsDetails.TextMessage;

            Debug.WriteLine(smsTextMessage.Body);

            //            smsTextMessage.

            smsDetails.Accept();
                      
            //settings.Values[taskName] = notification.Content;
        }
    }
}
