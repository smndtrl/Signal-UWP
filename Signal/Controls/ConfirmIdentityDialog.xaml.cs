using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Signal.Database;
using Signal.Models;
using GalaSoft.MvvmLight.Command;
using libtextsecure.messages;
using libtextsecure.push;
using Signal.Tasks;
using Signal.Util;
using TextSecure;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Signal.Controls
{
    public sealed partial class ConfirmIdentityDialog : ContentDialog
    {

        private MessageRecord _messageRecord;
        private IdentityKeyMismatch mismatch;

        public ConfirmIdentityDialog()
        {
            
        }

        public ConfirmIdentityDialog(MessageRecord record)
        {
            this.InitializeComponent();
            this._messageRecord = record;

            if (record.MismatchedIdentities == null) CancelCommand.Execute(null);

            if (record.MismatchedIdentities != null) mismatch = record.MismatchedIdentities[0];
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
           /* Log.Debug($"OnApplyTemplate");

            _statusView = GetTemplateChild("DeliveryStatusView") as DeliveryStatusView; ;*/
              
        }

        private void processMessageRecord(MessageRecord record)
        {
            if (record.IsOutgoing) processOutgoingMessageRecord(record);
            else processIncomingMessageRecord(record);
        }

        private void processPendingMessageRecords(long threadId, IdentityKeyMismatch mismatch)
        {
            var messageDatabase = DatabaseFactory.getMessageDatabase();
            var conflictMessages = messageDatabase.getIdentityConflictMessagesForThread(threadId);
 

            foreach (var record in conflictMessages)
            {
                foreach (var recordMismatch in record.MismatchedIdentities)
                {
                    Log.Debug($"This: {mismatch.IdentityKey.getFingerprint()} That:{recordMismatch.IdentityKey.getFingerprint()}");
                    if (mismatch.Equals(recordMismatch))
                    {
                        processMessageRecord(record);
                    }
                }
            }

        }

        private void processOutgoingMessageRecord(MessageRecord record)
        {
            var messageDatabase = DatabaseFactory.getMessageDatabase();
            /*MmsDatabase mmsDatabase = DatabaseFactory.getMmsDatabase());
            MmsAddressDatabase mmsAddressDatabase = DatabaseFactory.getMmsAddressDatabase();*/

            /*if (messageRecord.isMms())
            {
                mmsDatabase.removeMismatchedIdentity(messageRecord.MismatchedIdentities,
                                                     mismatch.RecipientId,
                                                     mismatch.IdentityKey);

                var recipients = mmsAddressDatabase.getRecipientsForId(messageRecord.MessageId);

                if (recipients.isGroupRecipient()) MessageSender.resendGroupMessage(, masterSecret, messageRecord, mismatch.getRecipientId());
                else MessageSender.resend(getContext(), masterSecret, messageRecord);
            }
            else {*/
            messageDatabase.RemoveMismatchedIdentity(record.MessageId,
                                                     mismatch.RecipientId,
                                                     mismatch.IdentityKey);

                MessageSender.resend(record);
           // }
        }

        private void processIncomingMessageRecord(MessageRecord record)
        {
            try
            {
                PushDatabase pushDatabase = DatabaseFactory.getPushDatabase();
                var messageDatabase = DatabaseFactory.getMessageDatabase();

                messageDatabase.RemoveMismatchedIdentity(record.MessageId,
                                                     mismatch.RecipientId,
                                                     mismatch.IdentityKey);

                TextSecureEnvelope envelope = new TextSecureEnvelope((uint)TextSecureProtos.Envelope.Types.Type.PREKEY_BUNDLE,
                                                                     record.IndividualRecipient.getNumber(),
                                                                     (uint)record.RecipientDeviceId, "",
                                                                     (ulong)TimeUtil.GetUnixTimestamp(record.DateSent),
                                                                     Base64.decode(record.Body.Body),
                                                                     null);

                long pushId = pushDatabase.Insert(envelope);

                var task = new PushDecryptTask(pushId, record.MessageId, record.IndividualRecipient.getNumber());
                App.Current.Worker.AddTaskActivities(task);
            }
            catch (IOException e)
            {
                throw new Exception();
            }
        }

        private RelayCommand _acceptCommand;

        public RelayCommand AcceptCommand
        {
            get
            {
                return _acceptCommand ?? (_acceptCommand = new RelayCommand(
                   () =>
                   {
                       var identityDatabase = DatabaseFactory.getIdentityDatabase();

                       identityDatabase.SaveIdentity(mismatch.RecipientId, mismatch.IdentityKey);

                       processMessageRecord(_messageRecord);
                       processPendingMessageRecords(_messageRecord.ThreadId, mismatch);
                   },
                   () => true));
            }
        }

        private RelayCommand _cancelCommand;

        public RelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new RelayCommand(
                   () =>
                   {
                       return;
                   },
                   () => true));
            }
        }

    }
}
