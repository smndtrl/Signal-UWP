using libaxolotl;
using libaxolotl.groups;
using libaxolotl.groups.state;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libaxolotl_test.groups
{
    public class InMemorySenderKeyStore : SenderKeyStore
    {
        private readonly IDictionary<SenderKeyName, SenderKeyRecord> store = new Dictionary<SenderKeyName, SenderKeyRecord>();

        public void storeSenderKey(SenderKeyName senderKeyName, SenderKeyRecord record)
        {
            int hashcode = senderKeyName.GetHashCode();

            if(store.ContainsKey(senderKeyName))
            {
                store.Remove(senderKeyName);
            }
            store.Add(senderKeyName, record);
        }

        public SenderKeyRecord loadSenderKey(SenderKeyName senderKeyName)
        {
            try
            {
                SenderKeyRecord record;
                store.TryGetValue(senderKeyName, out record);

                if (record == null)
                {
                    return new SenderKeyRecord();
                }
                else
                {
                    return new SenderKeyRecord(record.serialize());
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
