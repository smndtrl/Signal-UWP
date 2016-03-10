using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libaxolotl;
using libaxolotl.ecc;
using Newtonsoft.Json;
using Signal.Util;
using SQLite.Net.Attributes;

namespace Signal.Database
{
    public class IdentityKeyMismatch
    {
        public long RecipientId { get; set; }

        [JsonConverter(typeof(IdentityKeySerializer))]
        public IdentityKey IdentityKey { get; set; }
        /*{
            get { return new IdentityKey(new DjbECPublicKey(Key)); }
            set { Key = ((value.getPublicKey()) as DjbECPublicKey).getPublicKey(); }
        }*/

        // private byte[] Key { get; set; }

        public IdentityKeyMismatch()
        {
        }

        public IdentityKeyMismatch(long recipientId, IdentityKey identityKey)
        {
            this.RecipientId = recipientId;
            this.IdentityKey = identityKey;

            /*
                        var ecKey = identityKey.getPublicKey();
                        var key = ecKey as DjbECPublicKey;
                        if (key != null) Key = key.getPublicKey();*/

        }

        public override bool Equals(object other)
        {
            if (other == null) return false;
            if (!(other is IdentityKeyMismatch)) return false;

            IdentityKeyMismatch that = (IdentityKeyMismatch)other;
            return this.RecipientId.Equals(that.RecipientId) && this.IdentityKey.serialize().Equals(that.IdentityKey.serialize());
        }
    }
}
