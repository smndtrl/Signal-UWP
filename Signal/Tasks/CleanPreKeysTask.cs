using libaxolotl;
using libaxolotl.state;
using libtextsecure.push;
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
    public sealed class CleanPreKeysTask : UntypedTaskActivity
    {
        private static readonly int ARCHIVE_AGE_DAYS = 15;

        //@Inject transient TextSecureAccountManager accountManager;
        //@Inject transient SignedPreKeyStoreFactory signedPreKeyStoreFactory;

        public CleanPreKeysTask()
        {
            /*super(context, JobParameters.newBuilder()
                                        .withGroupId(CleanPreKeysJob.class.getSimpleName())
                                .withRequirement(new MasterSecretRequirement(context))
                                .withRetryCount(5)
                                .create());*/
        }

        public override void onAdded()
        {
            throw new NotImplementedException("CleanPreKeysTask onAdded");
        }

        protected override string Execute()
        {
            throw new NotImplementedException("CleanPreKeysTask Execute");
        }
        /*
protected override async Task<string> Execute()
{
   try
   {
       SignedPreKeyStore signedPreKeyStore = signedPreKeyStoreFactory.create(masterSecret);
       SignedPreKeyEntity currentSignedPreKey = await App.Current.accountManager.getSignedPreKey();

       if (currentSignedPreKey == null) return "";

       SignedPreKeyRecord currentRecord = signedPreKeyStore.loadSignedPreKey(currentSignedPreKey.getKeyId());
       List<SignedPreKeyRecord> allRecords = signedPreKeyStore.loadSignedPreKeys();
       LinkedList<SignedPreKeyRecord> oldRecords = removeRecordFrom(currentRecord, allRecords);

       Collections.sort(oldRecords, new SignedPreKeySorter());

       //Log.w(TAG, "Old signed prekey record count: " + oldRecords.size());

       bool foundAgedRecord = false;

       foreach (SignedPreKeyRecord oldRecord in oldRecords)
       {
           long archiveDuration = System.currentTimeMillis() - oldRecord.getTimestamp();

           if (archiveDuration >= TimeUnit.DAYS.toMillis(ARCHIVE_AGE_DAYS))
           {
               if (!foundAgedRecord)
               {
                   foundAgedRecord = true;
               }
               else
               {
                   Log.w(TAG, "Removing signed prekey record: " + oldRecord.getId() + " with timestamp: " + oldRecord.getTimestamp());
                   signedPreKeyStore.removeSignedPreKey(oldRecord.getId());
               }
           }
       }
   }
   catch (InvalidKeyIdException e)
   {
       //Log.w(TAG, e);
   }

   return "";
}





private LinkedList<SignedPreKeyRecord> removeRecordFrom(SignedPreKeyRecord currentRecord,
                                                       List<SignedPreKeyRecord> records)

{
   LinkedList<SignedPreKeyRecord> others = new LinkedList<SignedPreKeyRecord>();

   foreach (SignedPreKeyRecord record in records)
   {
       if (record.getId() != currentRecord.getId())
       {
           others.add(record);
       }
   }

   return others;
}

private static class SignedPreKeySorter implements Comparator<SignedPreKeyRecord> {
   public int compare(SignedPreKeyRecord lhs, SignedPreKeyRecord rhs)
{
   if (lhs.getTimestamp() > rhs.getTimestamp()) return -1;
   else if (lhs.getTimestamp() < rhs.getTimestamp()) return 1;
   else return 0;
}

protected override string Execute()
{
   throw new NotImplementedException();
}

public override void onAdded()
{
   throw new NotImplementedException();
}
}*/
    }
}
