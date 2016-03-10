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
using System.Text;
using libaxolotl.util;
using libtextsecure.messages;
using libtextsecure.push;
using Signal.Util;
using Strilanc.Value;
using TextSecure.util;

namespace Signal.Messages
{
    public class IncomingTextMessage
    {

        /*public static readonly Parcelable.Creator<IncomingTextMessage> CREATOR = new Parcelable.Creator<IncomingTextMessage>()
        {

    public IncomingTextMessage createFromParcel(Parcel in)
        {
            return new IncomingTextMessage(in);
        }

        @Override
    public IncomingTextMessage[] newArray(int size)
        {
            return new IncomingTextMessage[size];
        }
    };*/

        public string Message { get; }
        private string Sender { get; }
        private uint SenderDeviceId { get; }
        private int Protocol { get; }
        private string ServiceCenterAddress { get; }
        private bool ReplyPathPresent { get; }
        private string PseudoSubject { get; }
        public ulong SentTimestampMillis { get; }
        public string GroupId { get; }
        private bool Push { get; }

        /*public IncomingTextMessage(SmsMessage message)
        {
            this.message = message.getDisplayMessageBody();
            this.sender = message.getDisplayOriginatingAddress();
            this.senderDeviceId = TextSecureAddress.DEFAULT_DEVICE_ID;
            this.protocol = message.getProtocolIdentifier();
            this.serviceCenterAddress = message.getServiceCenterAddress();
            this.replyPathPresent = message.isReplyPathPresent();
            this.pseudoSubject = message.getPseudoSubject();
            this.sentTimestampMillis = message.getTimestampMillis();
            this.groupId = null;
            this.push = false;
        }*/

        public IncomingTextMessage(String sender, uint senderDeviceId, ulong sentTimestampMillis,
                                   String encodedBody, May<TextSecureGroup> group)
        {
            this.Message = encodedBody;
            this.Sender = sender;
            this.SenderDeviceId = senderDeviceId;
            this.Protocol = 31337;
            this.ServiceCenterAddress = "GCM";
            this.ReplyPathPresent = true;
            this.PseudoSubject = "";
            this.SentTimestampMillis = sentTimestampMillis;
            this.Push = true;

            if (group.HasValue)
            {
                this.GroupId = GroupUtil.getEncodedId(group.ForceGetValue().getGroupId());
            }
            else
            {
                this.GroupId = null;
            }
        }

        /* public IncomingTextMessage(Parcel in)
         {
             this.message = in.readString();
             this.sender = in.readString();
             this.senderDeviceId = in.readInt();
             this.protocol = in.readInt();
             this.serviceCenterAddress = in.readString();
             this.replyPathPresent = (in.readInt() == 1);
             this.pseudoSubject = in.readString();
             this.sentTimestampMillis = in.readLong();
             this.groupId = in.readString();
             this.push = (in.readInt() == 1);
         }*/

        public IncomingTextMessage(IncomingTextMessage message, String newBody)
        {
            this.Message = newBody;
            this.Sender = message.getSender();
            this.SenderDeviceId = message.getSenderDeviceId();
            this.Protocol = message.getProtocol();
            this.ServiceCenterAddress = message.getServiceCenterAddress();
            this.ReplyPathPresent = message.isReplyPathPresent();
            this.PseudoSubject = message.getPseudoSubject();
            this.SentTimestampMillis = message.SentTimestampMillis;
            this.GroupId = message.GroupId;
            this.Push = message.IsPush;
        }

        public IncomingTextMessage(List<IncomingTextMessage> fragments)
        {
            StringBuilder body = new StringBuilder();

            foreach (IncomingTextMessage message in fragments)
            {
                body.Append(message.getMessageBody());
            }

            this.Message = body.ToString();
            this.Sender = fragments[0].getSender();
            this.SenderDeviceId = fragments[0].getSenderDeviceId();
            this.Protocol = fragments[0].getProtocol();
            this.ServiceCenterAddress = fragments[0].getServiceCenterAddress();
            this.ReplyPathPresent = fragments[0].isReplyPathPresent();
            this.PseudoSubject = fragments[0].getPseudoSubject();
            this.SentTimestampMillis = fragments[0].SentTimestampMillis;
            this.GroupId = fragments[0].GroupId;
            this.Push = fragments[0].IsPush;
        }

        protected IncomingTextMessage(String sender, String groupId)
        {
            this.Message = "";
            this.Sender = sender;
            this.SenderDeviceId = TextSecureAddress.DEFAULT_DEVICE_ID;
            this.Protocol = 31338;
            this.ServiceCenterAddress = "Outgoing";
            this.ReplyPathPresent = true;
            this.PseudoSubject = "";
            this.SentTimestampMillis = (ulong)TimeUtil.GetUnixTimestampMillis();
            this.GroupId = groupId;
            this.Push = true;
        }


        public String getPseudoSubject()
        {
            return PseudoSubject;
        }

        public String getMessageBody()
        {
            return Message;
        }

        public IncomingTextMessage withMessageBody(String message)
        {
            return new IncomingTextMessage(this, message);
        }

        public String getSender()
        {
            return Sender;
        }

        public uint getSenderDeviceId()
        {
            return SenderDeviceId;
        }

        public int getProtocol()
        {
            return Protocol;
        }

        public String getServiceCenterAddress()
        {
            return ServiceCenterAddress;
        }

        public bool isReplyPathPresent()
        {
            return ReplyPathPresent;
        }

        public bool isSecureMessage()
        {
            return false;
        }

        public bool isPreKeyBundle()
        {
            return false;
        }

        public bool IsEndSession => false;

        public bool IsPush => Push;

        public bool IsGroup => false;

        //@Override
        public int describeContents()
        {
            return 0;
        }

        //@Override
        /* public void writeToParcel(Parcel out, int flags)
               {
           out.writeString(message);
           out.writeString(sender);
           out.writeInt(senderDeviceId);
           out.writeInt(protocol);
           out.writeString(serviceCenterAddress);
           out.writeInt(replyPathPresent ? 1 : 0);
           out.writeString(pseudoSubject);
           out.writeLong(sentTimestampMillis);
           out.writeString(groupId);
           out.writeInt(push ? 1 : 0);
               }*/
    }
}
