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

using libaxolotl.state;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libaxolotl;
using SQLite;
using System.IO;
using TextSecure.database;
using TextSecure.util;
using TextSecure.recipient;
using Signal.Database;
using SQLite.Net;

namespace TextSecure.crypto.storage
{
    public class TextSecureIdentityKeyStore : IdentityKeyStore
    {
        private SQLiteConnection conn;
        public TextSecureIdentityKeyStore(SQLiteConnection conn)
        {
            this.conn = conn;
        }

        public IdentityKeyPair GetIdentityKeyPair()
        {
            return IdentityKeyUtil.GetIdentityKeyPair();
        }

        public uint GetLocalRegistrationId()
        {
            return (uint)TextSecurePreferences.GetLocalRegistrationId();
        }

        public bool SaveIdentity(string name, IdentityKey identityKey)
        {
            long recipientId = RecipientFactory.getRecipientsFromString(name, true).getPrimaryRecipient().getRecipientId();
            DatabaseFactory.getIdentityDatabase().SaveIdentity(recipientId, identityKey);
            return true;
        }

        public bool IsTrustedIdentity(string name, IdentityKey identityKey)
        {
            long recipientId = RecipientFactory.getRecipientsFromString(name, true).getPrimaryRecipient().getRecipientId();
            return DatabaseFactory.getIdentityDatabase()
                                  .IsValidIdentity(recipientId, identityKey); 
        }
    }
}
