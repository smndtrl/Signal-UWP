using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libaxolotl;
using libaxolotl.state;
using Signal.Tasks.Library;
using Signal.Util;
using TextSecure.crypto;
using TextSecure.util;

namespace Signal.Tasks
{
    public sealed class CreateSignedPreKeyTask : UntypedTaskActivity
    {
        public override void onAdded()
        {
           // throw new NotImplementedException();
        }

        protected override string Execute()
        {
            if (TextSecurePreferences.isSignedPreKeyRegistered())
            {
                Log.Warn("Signed prekey already registered...");
                return "";
            }

            IdentityKeyPair identityKeyPair = IdentityKeyUtil.GetIdentityKeyPair();
            var signedPreKeyRecord = PreKeyUtil.generateSignedPreKey(identityKeyPair);

            App.Current.accountManager.setSignedPreKey(signedPreKeyRecord);
            TextSecurePreferences.setSignedPreKeyRegistered(true);

            return "";
        }
    }
}
