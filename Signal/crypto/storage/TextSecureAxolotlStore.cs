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
using libtextsecure.push;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using libaxolotl;
using TextSecure.crypto.storage;

namespace TextSecure
{
    public class TextSecureAxolotlStore : AxolotlStore
    {
        private readonly PreKeyStore preKeyStore;
        private readonly SignedPreKeyStore signedPreKeyStore;
        private readonly IdentityKeyStore identityKeyStore;
        private readonly SessionStore sessionStore;

        public TextSecureAxolotlStore()
        {
            this.preKeyStore = new TextSecurePreKeyStore();
            this.signedPreKeyStore = new TextSecurePreKeyStore();
            this.identityKeyStore = new TextSecureIdentityKeyStore();
            this.sessionStore = new TextSecureSessionStore();
        }

        public bool containsPreKey(uint preKeyId)
        {
            return preKeyStore.containsPreKey(preKeyId);
        }

        public bool containsSession(AxolotlAddress address)
        {
            return sessionStore.containsSession(address);
        }

        public bool containsSignedPreKey(uint signedPreKeyId)
        {
            return signedPreKeyStore.containsSignedPreKey(signedPreKeyId);
        }

        public void deleteAllSessions(string name)
        {
            sessionStore.deleteAllSessions(name);
        }

        public void deleteSession(AxolotlAddress address)
        {
            sessionStore.deleteSession(address);
        }

        public IdentityKeyPair getIdentityKeyPair()
        {
            return identityKeyStore.getIdentityKeyPair();
        }

        public uint getLocalRegistrationId()
        {
            return identityKeyStore.getLocalRegistrationId();
        }

        public List<uint> getSubDeviceSessions(string name)
        {
            return sessionStore.getSubDeviceSessions(name);
        }

        public bool isTrustedIdentity(string name, IdentityKey identityKey)
        {
            return identityKeyStore.isTrustedIdentity(name, identityKey);
        }

        public PreKeyRecord loadPreKey(uint preKeyId)
        {
            return preKeyStore.loadPreKey(preKeyId);
        }

        public SessionRecord loadSession(AxolotlAddress address)
        {
            return sessionStore.loadSession(address);
        }

        public SignedPreKeyRecord loadSignedPreKey(uint signedPreKeyId)
        {
            return signedPreKeyStore.loadSignedPreKey(signedPreKeyId);
        }

        public List<SignedPreKeyRecord> loadSignedPreKeys()
        {
            return signedPreKeyStore.loadSignedPreKeys();
        }

        public void removePreKey(uint preKeyId)
        {
            preKeyStore.removePreKey(preKeyId);
        }

        public void removeSignedPreKey(uint signedPreKeyId)
        {
            signedPreKeyStore.removeSignedPreKey(signedPreKeyId);
        }

        public bool saveIdentity(string name, IdentityKey identityKey)
        {
            identityKeyStore.saveIdentity(name, identityKey);
            return true;
        }

        public void storePreKey(uint preKeyId, PreKeyRecord record)
        {
            preKeyStore.storePreKey(preKeyId, record);
        }

        public void storeSession(AxolotlAddress address, SessionRecord record)
        {
            sessionStore.storeSession(address, record);
        }

        public void storeSignedPreKey(uint signedPreKeyId, SignedPreKeyRecord record)
        {
            signedPreKeyStore.storeSignedPreKey(signedPreKeyId, record);
        }
    }
}
