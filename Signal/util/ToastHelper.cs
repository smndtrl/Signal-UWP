using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Notifications;
using Microsoft.QueryStringDotNET;
using NotificationsExtensions.Toasts;
using Signal.Models;
using TextSecure.recipient;

namespace Signal.Util
{
    public class ToastHelper
    {



        public static void NotifyMessageDeliveryFailed(Recipients recipients, long threadId)
        {
            var content = new ToastContent()
            {
                Launch = new QueryString()
                {
                    {"action", "viewConversation" },
                    { "threadId",  threadId.ToString() }
                }.ToString(),

                Visual = new ToastVisual()
                {
                    TitleText = new ToastText()
                    {
                        Text = $"Message delivery failed"
                    },
                },

                /*Audio = new ToastAudio()
                {
                    Src = new Uri("ms-winsoundevent:Notification.IM")
                }*/
            };

            var doc = content.GetXml();

            // Generate WinRT notification
            var noti = new ToastNotification(doc);
            ToastNotificationManager.CreateToastNotifier().Show(noti);
        }

        public static void NewMessage(MessageRecord message)
        {
            var content = new ToastContent()
            {
                Launch = new QueryString()
                {
                    {"action", "viewConversation" },
                    { "threadId",  message.ThreadId.ToString() } 
                }.ToString(),

                Visual = new ToastVisual()
                {
                    TitleText = new ToastText()
                    {
                        Text = $"{message.IndividualRecipient.ShortString} sent you a message"
                    },

                    BodyTextLine1 = new ToastText()
                    {
                        Text = $"{message.Body.Body}"
                    }
                },

                Actions = new ToastActionsCustom()
                {
                    Inputs =
                    {
                        new ToastTextBox("tbReply")
                        {
                            PlaceholderContent = "Type a response"
                        }
                    },

                    Buttons =
                    {
                        new ToastButton("reply", "reply")
                        {
                            ActivationType = ToastActivationType.Background,
                            ImageUri = "Assets/ic_done_all_white_18dp.png",
                            TextBoxId = "tbReply"
                        }
                    }
                },

                Audio = new ToastAudio()
                {
                    Src = new Uri("ms-winsoundevent:Notification.IM")
                }
            };

            var doc = content.GetXml();

            // Generate WinRT notification
            var noti = new ToastNotification(doc);
            ToastNotificationManager.CreateToastNotifier().Show(noti);
        }
    }
}
