using libaxolotl;
using libaxolotl.state;
using libtextsecure.crypto;
using libtextsecure.messages;
using libtextsecure.messages.multidevice;
using libtextsecure.push;
using Signal.Database;
using Strilanc.Value;
using System;
using System.Threading.Tasks;
using libaxolotl.protocol;
using libaxolotl.util;
using Signal.Messages;
using Signal.Tasks.Library;
using Signal.Util;
using TextSecure;
using TextSecure.crypto.storage;
using TextSecure.recipient;
using TextSecure.util;

namespace Signal.Tasks
{
    class PushDecryptTask : UntypedTaskActivity
    {
        private readonly long _pushMessageId;
        private readonly long _smsMessageId;


        public PushDecryptTask(long pushMessageId, string sender) : this(pushMessageId, -1, sender)
        {
        }

        public PushDecryptTask(long pushMessageId, long smsMessageId, string sender)
        {
            this._pushMessageId = pushMessageId;
            this._smsMessageId = smsMessageId;
        }

        public override void onAdded() { }

        protected override string Execute()
        {
            throw new NotImplementedException("PushDecryptTask Execute");
        }

        protected override async Task<string> ExecuteAsync()
        {
            PushDatabase database = DatabaseFactory.getPushDatabase();
            TextSecureEnvelope envelope = database.GetEnvelope(_pushMessageId);
            May<long> optionalSmsMessageId = _smsMessageId > 0 ? new May<long>(_smsMessageId) : May<long>.NoValue;

            handleMessage(envelope, optionalSmsMessageId);
            database.Delete(_pushMessageId);

            return "";
        }

        private void handleMessage(TextSecureEnvelope envelope, May<long> smsMessageId)
        {
            try
            {
                AxolotlStore axolotlStore = new TextSecureAxolotlStore();
                TextSecureAddress localAddress = new TextSecureAddress(TextSecurePreferences.getLocalNumber());
                TextSecureCipher cipher = new TextSecureCipher(localAddress, axolotlStore);

                TextSecureContent content = cipher.decrypt(envelope);

                if (content.getDataMessage().HasValue)
                {
                    TextSecureDataMessage message = content.getDataMessage().ForceGetValue();

                    if (message.isEndSession()) handleEndSessionMessage(envelope, message, smsMessageId);
                    else if (message.isGroupUpdate()) handleGroupMessage(envelope, message, smsMessageId);
                    else if (message.getAttachments().HasValue) handleMediaMessage(envelope, message, smsMessageId);
                    else handleTextMessage(envelope, message, smsMessageId);
                }
                /*else if (content.getSyncMessage().HasValue) TODO: SYNC enable
                {
                    TextSecureSyncMessage syncMessage = content.getSyncMessage().ForceGetValue();

                    if (syncMessage.getSent().HasValue) handleSynchronizeSentMessage(masterSecret, syncMessage.getSent().ForceGetValue(), smsMessageId);
                    else if (syncMessage.getRequest().HasValue) handleSynchronizeRequestMessage(masterSecret, syncMessage.getRequest().ForceGetValue());
                }*/

                if (envelope.isPreKeyWhisperMessage())
                {
                    App.Current.Worker.AddTaskActivities(new RefreshPreKeysTask());
                    //ApplicationContext.getInstance(context).getJobManager().add(new RefreshPreKeysJob(context));
                }
            }
            catch (InvalidVersionException e)
            {
                Log.Warn(e);
                handleInvalidVersionMessage(envelope, smsMessageId);
            }
            catch (InvalidMessageException/* | InvalidKeyIdException | InvalidKeyException | MmsException*/ e)
            {
                Log.Warn(e);
                handleCorruptMessage(envelope, smsMessageId);
            }
            catch (InvalidKeyIdException e)
            {
                Log.Warn(e);
                handleCorruptMessage(envelope, smsMessageId);
            }
            catch (InvalidKeyException e)
            {
                Log.Warn(e);
                handleCorruptMessage(envelope, smsMessageId);
            }
            catch (NoSessionException e)
            {
                Log.Warn(e);
                handleNoSessionMessage(envelope, smsMessageId);
            }
            catch (LegacyMessageException e)
            {
                Log.Warn(e);
                handleLegacyMessage(envelope, smsMessageId);
            }
            catch (DuplicateMessageException e)
            {
                Log.Warn(e);
                handleDuplicateMessage(envelope, smsMessageId);
            }
            catch (libaxolotl.exceptions.UntrustedIdentityException e)
            {
                Log.Warn(e);
                handleUntrustedIdentityMessage(envelope, smsMessageId);
            }
        }

        private void handleEndSessionMessage(/*MasterSecretUnion     masterSecret,*/
                                             TextSecureEnvelope envelope,
                                             TextSecureDataMessage message,
                                             May<long> smsMessageId)
        {
            var smsDatabase = DatabaseFactory.getTextMessageDatabase();//getEncryptingSmsDatabase(context);
            var incomingTextMessage = new IncomingTextMessage(envelope.getSource(),
                                                                                envelope.getSourceDevice(),
                                                                                message.getTimestamp(),
                                                                                "", May<TextSecureGroup>.NoValue);

            long threadId;

            if (!smsMessageId.HasValue)
            {
                IncomingEndSessionMessage incomingEndSessionMessage = new IncomingEndSessionMessage(incomingTextMessage);
                Pair<long, long> messageAndThreadId = smsDatabase.InsertMessageInbox(incomingEndSessionMessage);

                threadId = messageAndThreadId.second();
            }
            else
            {
                var messageId = smsMessageId.ForceGetValue();
                smsDatabase.MarkAsEndSession(messageId);
                threadId = smsDatabase.GetThreadIdForMessage(messageId);
            }

            SessionStore sessionStore = new TextSecureAxolotlStore();
            sessionStore.DeleteAllSessions(envelope.getSource());

            //SecurityEvent.broadcastSecurityUpdateEvent(context, threadId);
            //MessageNotifier.updateNotification(context, masterSecret.getMasterSecret().orNull(), threadId);
        }

        private void handleGroupMessage(TextSecureEnvelope envelope, TextSecureDataMessage message, May<long> smsMessageId)
        {
            //GroupMessageProcessor.process(envelope, message, false); // TODO: GROUP enable

            if (smsMessageId.HasValue)
            {
                DatabaseFactory.getTextMessageDatabase().DeleteThread(smsMessageId.ForceGetValue()); //getSmsDatabase(context).deleteMessage(smsMessageId.get());
            }
        }

        #region Synchronize

        private void handleSynchronizeSentMessage(TextSecureEnvelope envelope, SentTranscriptMessage message, May<long> smsMessageId) // throws MmsException
        {
            long threadId;

            if (message.getMessage().isGroupUpdate())
            {
                throw new NotImplementedException("GROUP handleSynchronizeSentMessage");
                //threadId = GroupMessageProcessor.process(context, masterSecret, envelope, message.getMessage(), true); // TODO: Group enable
            }
            else if (message.getMessage().getAttachments().HasValue)
            {
                threadId = handleSynchronizeSentMediaMessage(message, smsMessageId);
            }
            else {
                threadId = handleSynchronizeSentTextMessage(message, smsMessageId);
            }

            if (threadId == 0L) return;

            DatabaseFactory.getThreadDatabase().SetRead(threadId);
            //MessageNotifier.updateNotification(getContext(), masterSecret.getMasterSecret().orNull());
        }

        private void handleSynchronizeRequestMessage(RequestMessage message)
        {
            throw new NotImplementedException("handleSynchronizeRequestMessage");
            if (message.isContactsRequest())
            {
                /*ApplicationContext.getInstance(context)
                                  .getJobManager()
                                  .add(new MultiDeviceContactUpdateJob(getContext()));*/
            }

            if (message.isGroupsRequest())
            {
                /*ApplicationContext.getInstance(context)
                                  .getJobManager()
                                  .add(new MultiDeviceGroupUpdateJob(getContext()));*/
            }
        }

        private long handleSynchronizeSentTextMessage(SentTranscriptMessage message,
                                                May<long> smsMessageId)
        {
            var textMessageDatabase = DatabaseFactory.getTextMessageDatabase();//getEncryptingSmsDatabase(context);
            Recipients recipients = getSyncMessageDestination(message);
            String body = message.getMessage().getBody().HasValue ? message.getMessage().getBody().ForceGetValue() : "";
            OutgoingTextMessage outgoingTextMessage = new OutgoingTextMessage(recipients, body);

            long threadId = DatabaseFactory.getThreadDatabase().GetThreadIdForRecipients(recipients);
            long messageId = textMessageDatabase.InsertMessageOutbox(threadId, outgoingTextMessage, TimeUtil.GetDateTime(message.getTimestamp()));

            textMessageDatabase.MarkAsSent(messageId);
            textMessageDatabase.MarkAsPush(messageId);
            textMessageDatabase.MarkAsSecure(messageId);

            if (smsMessageId.HasValue)
            {
                textMessageDatabase.Delete(smsMessageId.ForceGetValue());
            }

            return threadId;
        }

        private long handleSynchronizeSentMediaMessage(SentTranscriptMessage message,
                                                  May<long> smsMessageId)
        // throws MmsException
        {
            throw new NotImplementedException("handleSynchronizeSentMediaMessage");
            /*MmsDatabase database = DatabaseFactory.getMmsDatabase();
            Recipients recipients = getSyncMessageDestination(message);
            OutgoingMediaMessage mediaMessage = new OutgoingMediaMessage(recipients, message.getMessage().getBody().orNull(),
                                                                  PointerAttachment.forPointers(masterSecret, message.getMessage().getAttachments()),
                                                                  message.getTimestamp(), ThreadDatabase.DistributionTypes.DEFAULT);

            mediaMessage = new OutgoingSecureMediaMessage(mediaMessage);

            long threadId = DatabaseFactory.getThreadDatabase(context).getThreadIdFor(recipients);
            long messageId = database.insertMessageOutbox(masterSecret, mediaMessage, threadId, false);

            database.markAsSent(messageId);
            database.markAsPush(messageId);

            for (DatabaseAttachment attachment : DatabaseFactory.getAttachmentDatabase(context).getAttachmentsForMessage(messageId))
            {
                ApplicationContext.getInstance(context)
                                  .getJobManager()
                                  .add(new AttachmentDownloadJob(context, messageId, attachment.getAttachmentId()));
            }

            if (smsMessageId.isPresent())
            {
                DatabaseFactory.getSmsDatabase(context).deleteMessage(smsMessageId.get());
            }

            return threadId;*/
        }

        private Recipients getSyncMessageDestination(SentTranscriptMessage message)
        {
            if (message.getMessage().getGroupInfo().HasValue)
            {
                throw new NotImplementedException("GROUP getSyncMessageDestination");
                //return RecipientFactory.getRecipientsFromString(GroupUtil.getEncodedId(message.getMessage().getGroupInfo().get().getGroupId()), false); // TODO: Group enable
            }
            else {
                return RecipientFactory.getRecipientsFromString(message.getDestination().ForceGetValue(), false);
            }
        }



        private void handleMediaMessage(TextSecureEnvelope envelope, TextSecureDataMessage message, May<long> smsMessageId) // throws MmsException
        {
            throw new NotImplementedException("handleMediaMessage");
            /*
            var database = DatabaseFactory.getMediaMessageDatabase(); //getMmsDatabase(context);
            String localNumber = TextSecurePreferences.getLocalNumber(context);
            IncomingMediaMessage mediaMessage = new IncomingMediaMessage(masterSecret, envelope.getSource(),
                                                                 localNumber, message.getTimestamp(),
                                                                 Optional.fromNullable(envelope.getRelay()),
                                                                 message.getBody(),
                                                                 message.getGroupInfo(),
                                                                 message.getAttachments());

            Pair<long, long> messageAndThreadId = database.insertSecureDecryptedMessageInbox(mediaMessage, -1);
            List<DatabaseAttachment> attachments = DatabaseFactory.getAttachmentDatabase(context).getAttachmentsForMessage(messageAndThreadId.first);

            for (DatabaseAttachment attachment : attachments)
            {
                ApplicationContext.getInstance(context)
                                  .getJobManager()
                                  .add(new AttachmentDownloadJob(context, messageAndThreadId.first,
                                                                 attachment.getAttachmentId()));
            }

            if (smsMessageId.isPresent())
            {
                DatabaseFactory.getSmsDatabase(context).deleteMessage(smsMessageId.get());
            }

            MessageNotifier.updateNotification(context, masterSecret.getMasterSecret().orNull(), messageAndThreadId.second);*/
        }


        private void handleTextMessage(/*@NonNull MasterSecretUnion masterSecret,*/
                                       TextSecureEnvelope envelope,
                                       TextSecureDataMessage message,
                                       May<long> smsMessageId)
        {
            var textMessageDatabase = DatabaseFactory.getTextMessageDatabase();
            String body = message.getBody().HasValue ? message.getBody().ForceGetValue() : "";

            /*Pair<Long, Long> messageAndThreadId;

            if (smsMessageId.hasValue)
            {
                messageAndThreadId = database.updateBundleMessageBody(masterSecret, smsMessageId.get(), body);
            }
            else
            {*/
            IncomingTextMessage textMessage = new IncomingTextMessage(envelope.getSource(),
                                                                      envelope.getSourceDevice(),
                                                                      message.getTimestamp(), body,
                                                                      message.getGroupInfo());

            textMessage = new IncomingEncryptedMessage(textMessage, body);
            var messageAndThreadId = textMessageDatabase.InsertMessageInbox(textMessage);
            /*}

            MessageNotifier.updateNotification(context, masterSecret.getMasterSecret().orNull(), messageAndThreadId.second);*/
        }
        private void handleInvalidVersionMessage(TextSecureEnvelope envelope,
                                            May<long> smsMessageId)
        {
            var smsDatabase = DatabaseFactory.getTextMessageDatabase(); //getEncryptingSmsDatabase(context);

            if (!smsMessageId.HasValue)
            {
                Pair<long, long> messageAndThreadId = insertPlaceholder(envelope);
                smsDatabase.MarkAsInvalidVersionKeyExchange(messageAndThreadId.first());
                //MessageNotifier.updateNotification(context, masterSecret.getMasterSecret().orNull(), messageAndThreadId.second);
            }
            else {
                smsDatabase.MarkAsInvalidVersionKeyExchange(smsMessageId.ForceGetValue());
            }
        }

        private void handleCorruptMessage(TextSecureEnvelope envelope,
                                     May<long> smsMessageId)
        {
            var smsDatabase = DatabaseFactory.getTextMessageDatabase(); //getEncryptingSmsDatabase(context);

            if (!smsMessageId.HasValue)
            {
                Pair<long, long> messageAndThreadId = insertPlaceholder(envelope);
                smsDatabase.MarkAsDecryptFailed(messageAndThreadId.first());
                //MessageNotifier.updateNotification(context, masterSecret.getMasterSecret().orNull(), messageAndThreadId.second);
            }
            else {
                smsDatabase.MarkAsDecryptFailed(smsMessageId.ForceGetValue());
            }
        }
        private void handleNoSessionMessage(TextSecureEnvelope envelope,
                                      May<long> smsMessageId)
        {
            var smsDatabase = DatabaseFactory.getTextMessageDatabase(); //getEncryptingSmsDatabase(context);

            if (!smsMessageId.HasValue)
            {
                Pair<long, long> messageAndThreadId = insertPlaceholder(envelope);
                smsDatabase.MarkAsNoSession(messageAndThreadId.first());
                //MessageNotifier.updateNotification(context, masterSecret.getMasterSecret().orNull(), messageAndThreadId.second);
            }
            else {
                smsDatabase.MarkAsNoSession(smsMessageId.ForceGetValue());
            }
        }
        private void handleLegacyMessage(TextSecureEnvelope envelope, May<long> smsMessageId)
        {
            var smsDatabase = DatabaseFactory.getTextMessageDatabase(); //getEncryptingSmsDatabase(context);

            if (!smsMessageId.HasValue)
            {
                Pair<long, long> messageAndThreadId = insertPlaceholder(envelope);
                smsDatabase.MarkAsLegacyVersion(messageAndThreadId.first());
                //MessageNotifier.updateNotification(context, masterSecret.getMasterSecret().orNull(), messageAndThreadId.second);
            }
            else {
                smsDatabase.MarkAsLegacyVersion(smsMessageId.ForceGetValue());
            }
        }

        private void handleDuplicateMessage(TextSecureEnvelope envelope, May<long> smsMessageId)
        {
            // Let's start ignoring these now
            //    SmsDatabase smsDatabase = DatabaseFactory.getEncryptingSmsDatabase(context);
            //
            //    if (smsMessageId <= 0) {
            //      Pair<Long, Long> messageAndThreadId = insertPlaceholder(masterSecret, envelope);
            //      smsDatabase.markAsDecryptDuplicate(messageAndThreadId.first);
            //      MessageNotifier.updateNotification(context, masterSecret, messageAndThreadId.second);
            //    } else {
            //      smsDatabase.markAsDecryptDuplicate(smsMessageId);
            //    }
        }


        private void handleUntrustedIdentityMessage(TextSecureEnvelope envelope, May<long> smsMessageId)
        {
            try
            {
                var database = DatabaseFactory.getTextMessageDatabase(); //getEncryptingSmsDatabase(context);
                Recipients recipients = RecipientFactory.getRecipientsFromString(envelope.getSource(), false);
                long recipientId = recipients.getPrimaryRecipient().getRecipientId();
                PreKeyWhisperMessage whisperMessage = new PreKeyWhisperMessage(envelope.getLegacyMessage());
                IdentityKey identityKey = whisperMessage.getIdentityKey();
                String encoded = Base64.encodeBytes(envelope.getLegacyMessage());
                IncomingTextMessage textMessage = new IncomingTextMessage(envelope.getSource(), envelope.getSourceDevice(),
                                                                               envelope.getTimestamp(), encoded,
                                                                               May<TextSecureGroup>.NoValue);

                if (!smsMessageId.HasValue)
                {
                    IncomingPreKeyBundleMessage bundleMessage = new IncomingPreKeyBundleMessage(textMessage, encoded);
                    Pair<long, long> messageAndThreadId = database.InsertMessageInbox(bundleMessage);

                    database.SetMismatchedIdentity(messageAndThreadId.first(), recipientId, identityKey);
                    //MessageNotifier.updateNotification(context, masterSecret.getMasterSecret().orNull(), messageAndThreadId.second);
                }
                else
                {
                    var messageId = smsMessageId.ForceGetValue();
                    database.UpdateMessageBody(messageId, encoded);
                    database.MarkAsPreKeyBundle(messageId);
                    database.SetMismatchedIdentity(messageId, recipientId, identityKey);
                }
            }
            catch (InvalidMessageException e)
            {
                throw new InvalidOperationException(e.Message);
            }
            catch (InvalidVersionException e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        #endregion

        private Pair<long, long> insertPlaceholder(TextSecureEnvelope envelope)
        {
            var database = DatabaseFactory.getTextMessageDatabase(); //getEncryptingSmsDatabase(context);
            IncomingTextMessage textMessage = new IncomingTextMessage(envelope.getSource(), envelope.getSourceDevice(),
                                                                        envelope.getTimestamp(), "",
                                                                        May<TextSecureGroup>.NoValue);

            textMessage = new IncomingEncryptedMessage(textMessage, "");
            return database.InsertMessageInbox(textMessage);
        }
    }

}