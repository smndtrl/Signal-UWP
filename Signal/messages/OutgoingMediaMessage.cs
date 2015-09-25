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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.recipient;

namespace TextSecure.messages
{
    public class OutgoingMediaMessage
    {

        private Recipients recipients;
        //protected PduBody body;
        private int distributionType;

        /*public OutgoingMediaMessage(Recipients recipients, PduBody body,
                                    String message, int distributionType)
        {
            this.recipients = recipients;
            this.body = body;
            this.distributionType = distributionType;

            if (!TextUtils.isEmpty(message))
            {
                this.body.addPart(new TextSlide(context, message).getPart());
            }
        }

        public OutgoingMediaMessage(Context context, Recipients recipients, SlideDeck slideDeck,
                                    String message, int distributionType)
        {
            this(context, recipients, slideDeck.toPduBody(), message, distributionType);
        }

        public OutgoingMediaMessage(Context context, MasterSecretUnion masterSecret,
                                    Recipients recipients, List<TextSecureAttachment> attachments,
                                    String message)
        {
            this(context, recipients, pduBodyFor(masterSecret, attachments), message,
                 ThreadDatabase.DistributionTypes.CONVERSATION);
        }*/

        public OutgoingMediaMessage(OutgoingMediaMessage that)
        {
            this.recipients = that.getRecipients();
            //this.body = that.body;
            this.distributionType = that.distributionType;
        }

        public Recipients getRecipients()
        {
            return recipients;
        }

        /*public PduBody getPduBody()
        {
            return body;
        }*/

        public int getDistributionType()
        {
            return distributionType;
        }

        public bool isSecure()
        {
            return false;
        }

        public bool isGroup()
        {
            return false;
        }
        /*
        private static PduBody pduBodyFor(MasterSecretUnion masterSecret, List<TextSecureAttachment> attachments)
        {
            PduBody body = new PduBody();

            for (TextSecureAttachment attachment : attachments)
            {
                if (attachment.isPointer())
                {
                    PduPart media = new PduPart();
                    String encryptedKey = MediaKey.getEncrypted(masterSecret, attachment.asPointer().getKey());

                    media.setContentType(Util.toIsoBytes(attachment.getContentType()));
                    media.setContentLocation(Util.toIsoBytes(String.valueOf(attachment.asPointer().getId())));
                    media.setContentDisposition(Util.toIsoBytes(encryptedKey));

                    body.addPart(media);
                }
            }

            return body;
        }*/

    }
}
