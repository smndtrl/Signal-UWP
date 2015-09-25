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

namespace libaxolotl.state.impl
{
    public class InMemoryAxolotlStore : AxolotlStore
    {

        private readonly InMemoryPreKeyStore preKeyStore = new InMemoryPreKeyStore();
        private readonly InMemorySessionStore sessionStore = new InMemorySessionStore();
        private readonly InMemorySignedPreKeyStore signedPreKeyStore = new InMemorySignedPreKeyStore();

        private readonly InMemoryIdentityKeyStore identityKeyStore;

        public InMemoryAxolotlStore(IdentityKeyPair identityKeyPair, uint registrationId)
        {
            this.identityKeyStore = new InMemoryIdentityKeyStore(identityKeyPair, registrationId);
        }


        public IdentityKeyPair getIdentityKeyPair()
        {
            return identityKeyStore.getIdentityKeyPair();
        }


        public uint getLocalRegistrationId()
        {
            return identityKeyStore.getLocalRegistrationId();
        }


        public bool saveIdentity(String name, IdentityKey identityKey)
        {
            identityKeyStore.saveIdentity(name, identityKey);
            return true;
        }


        public bool isTrustedIdentity(String name, IdentityKey identityKey)
        {
            return identityKeyStore.isTrustedIdentity(name, identityKey);
        }


        public PreKeyRecord loadPreKey(uint preKeyId)
        {
            return preKeyStore.loadPreKey(preKeyId);
        }


        public void storePreKey(uint preKeyId, PreKeyRecord record)
        {
            preKeyStore.storePreKey(preKeyId, record);
        }


        public bool containsPreKey(uint preKeyId)
        {
            return preKeyStore.containsPreKey(preKeyId);
        }


        public void removePreKey(uint preKeyId)
        {
            preKeyStore.removePreKey(preKeyId);
        }


        public SessionRecord loadSession(AxolotlAddress address)
        {
            return sessionStore.loadSession(address);
        }


        public List<uint> getSubDeviceSessions(String name)
        {
            return sessionStore.getSubDeviceSessions(name);
        }


        public void storeSession(AxolotlAddress address, SessionRecord record)
        {
            sessionStore.storeSession(address, record);
        }


        public bool containsSession(AxolotlAddress address)
        {
            return sessionStore.containsSession(address);
        }


        public void deleteSession(AxolotlAddress address)
        {
            sessionStore.deleteSession(address);
        }


        public void deleteAllSessions(String name)
        {
            sessionStore.deleteAllSessions(name);
        }


        public SignedPreKeyRecord loadSignedPreKey(uint signedPreKeyId)
        {
            return signedPreKeyStore.loadSignedPreKey(signedPreKeyId);
        }


        public List<SignedPreKeyRecord> loadSignedPreKeys()
        {
            return signedPreKeyStore.loadSignedPreKeys();
        }


        public void storeSignedPreKey(uint signedPreKeyId, SignedPreKeyRecord record)
        {
            signedPreKeyStore.storeSignedPreKey(signedPreKeyId, record);
        }


        public bool containsSignedPreKey(uint signedPreKeyId)
        {
            return signedPreKeyStore.containsSignedPreKey(signedPreKeyId);
        }


        public void removeSignedPreKey(uint signedPreKeyId)
        {
            signedPreKeyStore.removeSignedPreKey(signedPreKeyId);
        }
    }
}
