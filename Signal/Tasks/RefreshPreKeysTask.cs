using libaxolotl;
using libaxolotl.state;
using Signal.Tasks.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.crypto;
using TextSecure.util;

namespace Signal.Tasks
{
    public sealed class RefreshPreKeysTask : UntypedTaskActivity
    {
        //@Inject transient TextSecureAccountManager accountManager;

        private static readonly int PREKEY_MINIMUM = 10;


        public RefreshPreKeysTask()
        {
            /*super(context, JobParameters.newBuilder()
                                        .withGroupId(RefreshPreKeysJob.class.getSimpleName())
                                .withRequirement(new NetworkRequirement(context))
                                .withRequirement(new MasterSecretRequirement(context))
                                .withRetryCount(5)
                                .create());*/
        }

        public override void onAdded()
        {
            throw new NotImplementedException("RefreshPreKeysTask onAdded");
        }

        protected override string Execute()
        {
            throw new NotImplementedException("RefreshPreKeysTask Execute");
        }

        protected override async Task<string> ExecuteAsync()
        {
            //if (!TextSecurePreferences.isPushRegistered()) return;

            int availableKeys = await App.Current.accountManager.getPreKeysCount();

            if (availableKeys >= PREKEY_MINIMUM && TextSecurePreferences.isSignedPreKeyRegistered())
            {
                Debug.WriteLine("Available keys sufficient: " + availableKeys);
                return "";
            }

            List<PreKeyRecord> preKeyRecords = await PreKeyUtil.generatePreKeys(/*context, masterSecret*/);
            PreKeyRecord lastResortKeyRecord = await PreKeyUtil.generateLastResortKey(/*context, masterSecret*/);
            IdentityKeyPair identityKey = IdentityKeyUtil.GetIdentityKeyPair(/*context, masterSecret*/);
            SignedPreKeyRecord signedPreKeyRecord = await PreKeyUtil.generateSignedPreKey(/*context, masterSecret, */identityKey);

            Debug.WriteLine("Registering new prekeys...");

            await App.Current.accountManager.setPreKeys(identityKey.getPublicKey(), lastResortKeyRecord, signedPreKeyRecord, preKeyRecords);

            TextSecurePreferences.setSignedPreKeyRegistered(true);

            App.Current.Worker.AddTaskActivities(new CleanPreKeysTask());

            return "";
        }

        /*@Override
          public boolean onShouldRetryThrowable(Exception exception)
        {
            if (exception instanceof NonSuccessfulResponseCodeException) return false;
            if (exception instanceof PushNetworkException)               return true;

            return false;
        }

        @Override
          public void onCanceled()
        {

        }*/
    }
}
