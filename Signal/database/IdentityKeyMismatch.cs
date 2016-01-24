using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libaxolotl;

namespace Signal.Database
{
    public class IdentityKeyMismatch
    {
        public long RecipientId { get; set; }
        public IdentityKey IdentityKey { get; set; }


        public IdentityKeyMismatch() { }

        public IdentityKeyMismatch(long recipientId, IdentityKey identityKey)
        {
            this.RecipientId = recipientId;
            this.IdentityKey = identityKey;
        }
    }
}
