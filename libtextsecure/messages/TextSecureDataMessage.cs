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
using Strilanc.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libtextsecure.messages
{
    /**
     * Represents a decrypted text secure message.
     */
    public class TextSecureDataMessage
    {

        private readonly ulong timestamp;
        private readonly May<LinkedList<TextSecureAttachment>> attachments;
        private readonly May<String> body;
        private readonly May<TextSecureGroup> group;

        private readonly bool endSession;

        /**
         * Construct a TextSecureMessage with a body and no attachments.
         *
         * @param timestamp The sent timestamp.
         * @param body The message contents.
         */
        /*public TextSecureDataMessage(long timestamp, String body)
                        : this(timestamp, new List<TextSecureAttachment>(), body)
              {

        }

        public TextSecureDataMessage(long timestamp, TextSecureAttachment attachment, String body)
                  : this(timestamp, new LinkedList<TextSecureAttachment>() { { add(attachment);
    }
}, body)
              {

        }*/

        /**
         * Construct a TextSecureMessage with a body and list of attachments.
         *
         * @param timestamp The sent timestamp.
         * @param attachments The attachments.
         * @param body The message contents.
         */
        public TextSecureDataMessage(ulong timestamp, LinkedList<TextSecureAttachment> attachments, String body)
            : this(timestamp, null, attachments, body)
        {

        }

        /**
         * Construct a TextSecure group message with attachments and body.
         *
         * @param timestamp The sent timestamp.
         * @param group The group information.
         * @param attachments The attachments.
         * @param body The message contents.
         */
        public TextSecureDataMessage(ulong timestamp, TextSecureGroup group, LinkedList<TextSecureAttachment> attachments, String body)
                            : this(timestamp, group, attachments, body, false)
        {

        }

        /**
         * Construct a TextSecureMessage.s
         *
         * @param timestamp The sent timestamp.
         * @param group The group information (or null if none).
         * @param attachments The attachments (or null if none).
         * @param body The message contents.
         * @param endSession Flag indicating whether this message should close a session.
         */
        public TextSecureDataMessage(ulong timestamp, TextSecureGroup group, LinkedList<TextSecureAttachment> attachments, String body, bool endSession)
        {
            this.timestamp = timestamp;
            this.body = new May<String>(body);
            this.group = group == null ? May<TextSecureGroup>.NoValue : new May<TextSecureGroup>(group);
            //this.syncContext = syncContext == null ? May<TextSecureSyncContext>.NoValue : new May<TextSecureSyncContext>(syncContext);
            this.endSession = endSession;

            if (attachments != null && !(attachments.Count == 0))
            {
                this.attachments = new May<LinkedList<TextSecureAttachment>>(attachments);
            }
            else
            {
                this.attachments = May<LinkedList<TextSecureAttachment>>.NoValue;
            }
        }

        public static TextSecureDataMessageBuilder newBuilder()
        {
            return new TextSecureDataMessageBuilder();
        }

        /**
         * @return The message timestamp.
         */
        public ulong getTimestamp()
        {
            return timestamp;
        }

        /**
         * @return The message attachments (if any).
         */
        public May<LinkedList<TextSecureAttachment>> getAttachments()
        {
            return attachments;
        }

        /**
         * @return The message body (if any).
         */
        public May<String> getBody()
        {
            return body;
        }

        /**
         * @return The message group info (if any).
         */
        public May<TextSecureGroup> getGroupInfo()
        {
            return group;
        }
        /*
        public May<TextSecureSyncContext> getSyncContext()
        {
            return syncContext;
        }*/

        public bool isEndSession()
        {
            return endSession;
        }

        public bool isGroupUpdate()
        {
            return group.HasValue && group.ForceGetValue().getType() != TextSecureGroup.Type.DELIVER;
        }

    }

    public class TextSecureDataMessageBuilder
    {

        private LinkedList<TextSecureAttachment> attachments = new LinkedList<TextSecureAttachment>();
        private ulong timestamp;
        private TextSecureGroup group;
        private String body;
        private bool endSession;

        public TextSecureDataMessageBuilder() { }

        public TextSecureDataMessageBuilder withTimestamp(ulong timestamp)
        {
            this.timestamp = timestamp;
            return this;
        }

        public TextSecureDataMessageBuilder asGroupMessage(TextSecureGroup group)
        {
            this.group = group;
            return this;
        }

        public TextSecureDataMessageBuilder withAttachment(TextSecureAttachment attachment)
        {
            this.attachments.AddLast(attachment);
            return this;
        }

        public TextSecureDataMessageBuilder withAttachments(List<TextSecureAttachment> attachments)
        {
            foreach (TextSecureAttachment attachment in attachments)
            {
                this.attachments.AddLast(attachment);
            }

            return this;
        }

        public TextSecureDataMessageBuilder withBody(String body)
        {
            this.body = body;
            return this;
        }

        public TextSecureDataMessageBuilder asEndSessionMessage()
        {
            this.endSession = true;
            return this;
        }

        public TextSecureDataMessageBuilder asEndSessionMessage(bool endSession)
        {
            this.endSession = endSession;
            return this;
        }

        public TextSecureDataMessage build()
        {
            if (timestamp == 0) timestamp = KeyHelper.getTime();
            return new TextSecureDataMessage(timestamp, group, attachments, body, endSession);
        }
    }
}
