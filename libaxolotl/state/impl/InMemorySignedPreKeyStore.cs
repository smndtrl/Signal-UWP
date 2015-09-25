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
    public class InMemorySignedPreKeyStore : SignedPreKeyStore
    {

        private readonly IDictionary<uint, byte[]> store = new Dictionary<uint, byte[]>();


        public SignedPreKeyRecord loadSignedPreKey(uint signedPreKeyId)
        {
            try
            {
                if (!store.ContainsKey(signedPreKeyId))
                {
                    throw new InvalidKeyIdException("No such signedprekeyrecord! " + signedPreKeyId);
                }

                byte[] record;
                store.TryGetValue(signedPreKeyId, out record);  // get()

                return new SignedPreKeyRecord(record);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        public List<SignedPreKeyRecord> loadSignedPreKeys()
        {
            try
            {
                List<SignedPreKeyRecord> results = new List<SignedPreKeyRecord>();

                foreach (byte[] serialized in store.Values) //values()
                {
                    results.Add(new SignedPreKeyRecord(serialized));
                }

                return results;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        public void storeSignedPreKey(uint signedPreKeyId, SignedPreKeyRecord record)
        {
            if (store.ContainsKey(signedPreKeyId)) // mimic Java HashMap
            {
                store.Remove(signedPreKeyId);
            }
            store.Add(signedPreKeyId, record.serialize());
        }


        public bool containsSignedPreKey(uint signedPreKeyId)
        {
            return store.ContainsKey(signedPreKeyId);
        }


        public void removeSignedPreKey(uint signedPreKeyId)
        {
            store.Remove(signedPreKeyId);
        }
    }
}
