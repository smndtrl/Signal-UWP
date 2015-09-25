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

using libtextsecure.messages.multidevice;
using Strilanc.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libtextsecure.messages
{
    public class TextSecureContent
    {

        private readonly May<TextSecureDataMessage> message;
        private readonly May<TextSecureSyncMessage> synchronizeMessage;

        public TextSecureContent()
        {
            this.message = May<TextSecureDataMessage>.NoValue;
            this.synchronizeMessage = May<TextSecureSyncMessage>.NoValue;
        }

        public TextSecureContent(TextSecureDataMessage message)
        {
            this.message = message == null ? May<TextSecureDataMessage>.NoValue : new May<TextSecureDataMessage>(message);
            this.synchronizeMessage = May<TextSecureSyncMessage>.NoValue;
        }

        public TextSecureContent(TextSecureSyncMessage synchronizeMessage)
        {
            this.message = May<TextSecureDataMessage>.NoValue;
            this.synchronizeMessage = synchronizeMessage == null ? May<TextSecureSyncMessage>.NoValue : new May<TextSecureSyncMessage>(synchronizeMessage);
        }

        public May<TextSecureDataMessage> getDataMessage()
        {
            return message;
        }

        public May<TextSecureSyncMessage> getSyncMessage()
        {
            return synchronizeMessage;
        }
    }

}
