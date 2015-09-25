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

using libaxolotl.util;
using libtextsecure.messages;
using libtextsecure.push;
using Strilanc.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.util;

namespace TextSecure.messages
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

        private readonly String message;
        private readonly String sender;
        private readonly uint senderDeviceId;
        private readonly int protocol;
        private readonly String serviceCenterAddress;
        private readonly bool replyPathPresent;
        private readonly String pseudoSubject;
        private readonly long sentTimestampMillis;
        private readonly String groupId;
        private readonly bool push;

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

        public IncomingTextMessage(String sender, uint senderDeviceId, long sentTimestampMillis,
                                   String encodedBody, May<TextSecureGroup> group)
        {
            this.message = encodedBody;
            this.sender = sender;
            this.senderDeviceId = senderDeviceId;
            this.protocol = 31337;
            this.serviceCenterAddress = "GCM";
            this.replyPathPresent = true;
            this.pseudoSubject = "";
            this.sentTimestampMillis = sentTimestampMillis;
            this.push = true;

            if (group.HasValue)
            {
                this.groupId = GroupUtil.getEncodedId(group.ForceGetValue().getGroupId());
            }
            else
            {
                this.groupId = null;
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
            this.message = newBody;
            this.sender = message.getSender();
            this.senderDeviceId = message.getSenderDeviceId();
            this.protocol = message.getProtocol();
            this.serviceCenterAddress = message.getServiceCenterAddress();
            this.replyPathPresent = message.isReplyPathPresent();
            this.pseudoSubject = message.getPseudoSubject();
            this.sentTimestampMillis = message.getSentTimestampMillis();
            this.groupId = message.getGroupId();
            this.push = message.isPush();
        }

        public IncomingTextMessage(List<IncomingTextMessage> fragments)
        {
            StringBuilder body = new StringBuilder();

            foreach (IncomingTextMessage message in fragments)
            {
                body.Append(message.getMessageBody());
            }

            this.message = body.ToString();
            this.sender = fragments[0].getSender();
            this.senderDeviceId = fragments[0].getSenderDeviceId();
            this.protocol = fragments[0].getProtocol();
            this.serviceCenterAddress = fragments[0].getServiceCenterAddress();
            this.replyPathPresent = fragments[0].isReplyPathPresent();
            this.pseudoSubject = fragments[0].getPseudoSubject();
            this.sentTimestampMillis = fragments[0].getSentTimestampMillis();
            this.groupId = fragments[0].getGroupId();
            this.push = fragments[0].isPush();
        }

        protected IncomingTextMessage(String sender, String groupId)
        {
            this.message = "";
            this.sender = sender;
            this.senderDeviceId = TextSecureAddress.DEFAULT_DEVICE_ID;
            this.protocol = 31338;
            this.serviceCenterAddress = "Outgoing";
            this.replyPathPresent = true;
            this.pseudoSubject = "";
            this.sentTimestampMillis = (long)KeyHelper.getTime();
            this.groupId = groupId;
            this.push = true;
        }

        public long getSentTimestampMillis()
        {
            return sentTimestampMillis;
        }

        public String getPseudoSubject()
        {
            return pseudoSubject;
        }

        public String getMessageBody()
        {
            return message;
        }

        public IncomingTextMessage withMessageBody(String message)
        {
            return new IncomingTextMessage(this, message);
        }

        public String getSender()
        {
            return sender;
        }

        public uint getSenderDeviceId()
        {
            return senderDeviceId;
        }

        public int getProtocol()
        {
            return protocol;
        }

        public String getServiceCenterAddress()
        {
            return serviceCenterAddress;
        }

        public bool isReplyPathPresent()
        {
            return replyPathPresent;
        }

        public bool isSecureMessage()
        {
            return false;
        }

        public bool isPreKeyBundle()
        {
            return false;
        }

        public bool isEndSession()
        {
            return false;
        }

        public bool isPush()
        {
            return push;
        }

        public String getGroupId()
        {
            return groupId;
        }

        public bool isGroup()
        {
            return false;
        }

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
