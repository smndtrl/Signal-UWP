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

using Strilanc.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libtextsecure.messages
{
    /**
 * Represents a received TextSecureMessage attachment "handle."  This
 * is a pointer to the actual attachment content, which needs to be
 * retrieved using {@link org.whispersystems.textsecure.api.TextSecureMessageReceiver#retrieveAttachment(TextSecureAttachmentPointer, java.io.File)}
 *
 * @author
 */
    class TextSecureAttachmentPointer : TextSecureAttachment
    {

        private readonly ulong id;
        private readonly byte[] key;
        private readonly May<String> relay;

        public TextSecureAttachmentPointer(ulong id, String contentType, byte[] key, String relay)
            : base(contentType)
        {
            this.id = id;
            this.key = key;
            this.relay = new May<String>(relay);
        }

        public ulong getId()
        {
            return id;
        }

        public byte[] getKey()
        {
            return key;
        }

        public override bool isStream()
        {
            return false;
        }


        public override bool isPointer()
        {
            return true;
        }

        public May<String> getRelay()
        {
            return relay;
        }
    }
}
