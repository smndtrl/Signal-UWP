using libaxolotl;
using libaxolotl.state;
using libtextsecure;
using libtextsecure.crypto;
using libtextsecure.messages;
using libtextsecure.messages.multidevice;
using libtextsecure.push;
using libtextsecure.util;
using Signal.database.models;
using Signal.Tasks.Library;
using Strilanc.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure;
using TextSecure.crypto.storage;
using TextSecure.database;
using TextSecure.messages;
using TextSecure.util;

namespace Signal.Tasks
{
    class PushDecryptTask : SendTask
    {
        private long messageId;

        public PushDecryptTask(long messageId, string sender)
        {
            this.messageId = messageId;
        }

        public override void onAdded()
        {
        }

        protected override string Execute()
        {
            throw new NotImplementedException("PushDecryptTask Execute");
        }

        protected override async Task<string> Execute()
        {
            /*PushDatabase database = DatabaseFactory.getPushDatabase();
            TextSecureEnvelope envelope = database.get(messageId);
            Optional<Long> optionalSmsMessageId = smsMessageId > 0 ? Optional.of(smsMessageId) :
                                                                         Optional.< Long > absent();

            //handleMessage(/*masterSecret,/envelope/*, optionalSmsMessageId);*/
            //database.delete(messageId);

            return "";
        }

        private void handleMessage(/*MasterSecretUnion masterSecret, */TextSecureEnvelope envelope/*, Optional<Long> smsMessageId*/)
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

                    if (message.isEndSession()) handleEndSessionMessage(/*masterSecret, */envelope, message/*, smsMessageId*/);
                    //else if (message.isGroupUpdate()) handleGroupMessage(masterSecret, envelope, message, smsMessageId);
                    //else if (message.getAttachments().isPresent()) handleMediaMessage(masterSecret, envelope, message, smsMessageId);
                    else handleTextMessage(/*masterSecret, */envelope, message/*, smsMessageId*/);
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
                //Log.w(TAG, e);
                //handleInvalidVersionMessage(masterSecret, envelope, smsMessageId);
            }
            /* catch (InvalidMessageException | InvalidKeyIdException | InvalidKeyException | MmsException e) {
                 Log.w(TAG, e);
                 handleCorruptMessage(masterSecret, envelope, smsMessageId);
             } catch (NoSessionException e)
             {
                 Log.w(TAG, e);
                 handleNoSessionMessage(masterSecret, envelope, smsMessageId);
             }
             catch (LegacyMessageException e)
             {
                 Log.w(TAG, e);
                 handleLegacyMessage(masterSecret, envelope, smsMessageId);
             }
             catch (DuplicateMessageException e)
             {
                 Log.w(TAG, e);
                 handleDuplicateMessage(masterSecret, envelope, smsMessageId);
             }
             catch (UntrustedIdentityException e)
             {
                 Log.w(TAG, e);
                 handleUntrustedIdentityMessage(masterSecret, envelope, smsMessageId);
             }*/
        }

        private void handleEndSessionMessage(/*MasterSecretUnion     masterSecret,*/
                                             TextSecureEnvelope envelope,
                                             TextSecureDataMessage message/*,
                                             Optional<Long>        smsMessageId*/)
        {
            /*EncryptingSmsDatabase smsDatabase = DatabaseFactory.getEncryptingSmsDatabase(context);
            IncomingTextMessage incomingTextMessage = new IncomingTextMessage(envelope.getSource(),
                                                                                envelope.getSourceDevice(),
                                                                                (long)message.getTimestamp(),
                                                                                "", May<TextSecureGroup>.NoValue);

            long threadId;

            if (!smsMessageId.isPresent())
            {
                IncomingEndSessionMessage incomingEndSessionMessage = new IncomingEndSessionMessage(incomingTextMessage);
                Pair<Long, Long> messageAndThreadId = smsDatabase.insertMessageInbox(masterSecret, incomingEndSessionMessage);

                threadId = messageAndThreadId.second;
            }
            else
            {
                smsDatabase.markAsEndSession(smsMessageId.get());
                threadId = smsDatabase.getThreadIdForMessage(smsMessageId.get());
            }

            SessionStore sessionStore = new TextSecureSessionStore();
            sessionStore.deleteAllSessions(envelope.getSource());*/

            //SecurityEvent.broadcastSecurityUpdateEvent(context, threadId);
            //MessageNotifier.updateNotification(context, masterSecret.getMasterSecret().orNull(), threadId);
        }

        /*private void handleGroupMessage(@NonNull MasterSecretUnion masterSecret,
                                        @NonNull TextSecureEnvelope envelope,
                                        @NonNull TextSecureDataMessage message,
                                        @NonNull Optional<Long> smsMessageId)
        {
            GroupMessageProcessor.process(context, masterSecret, envelope, message);

            if (smsMessageId.isPresent())
            {
                DatabaseFactory.getSmsDatabase(context).deleteMessage(smsMessageId.get());
            }
        }

        private void handleSynchronizeSentMessage(@NonNull MasterSecretUnion masterSecret,
                                                  @NonNull SentTranscriptMessage message,
                                                  @NonNull Optional<Long> smsMessageId)
      throws MmsException
        {
    if (message.getMessage().getAttachments().isPresent()) {
                handleSynchronizeSentMediaMessage(masterSecret, message, smsMessageId);
            } else {
                handleSynchronizeSentTextMessage(masterSecret, message, smsMessageId);
            }
        }

        private void handleSynchronizeRequestMessage(@NonNull MasterSecretUnion masterSecret,
                                                     @NonNull RequestMessage message)
        {
            if (message.isContactsRequest())
            {
                ApplicationContext.getInstance(context)
                                  .getJobManager()
                                  .add(new MultiDeviceContactUpdateJob(getContext()));
            }

            if (message.isGroupsRequest())
            {
                ApplicationContext.getInstance(context)
                                  .getJobManager()
                                  .add(new MultiDeviceGroupUpdateJob(getContext()));
            }
        }

        private void handleMediaMessage(@NonNull MasterSecretUnion masterSecret,
                                        @NonNull TextSecureEnvelope envelope,
                                        @NonNull TextSecureDataMessage message,
                                        @NonNull Optional<Long> smsMessageId)
      throws MmsException
        {
            MmsDatabase database     = DatabaseFactory.getMmsDatabase(context);
            String localNumber  = TextSecurePreferences.getLocalNumber(context);
            IncomingMediaMessage mediaMessage = new IncomingMediaMessage(masterSecret, envelope.getSource(),
                                                                 localNumber, message.getTimestamp(),
                                                                 Optional.fromNullable(envelope.getRelay()),
                                                                 message.getBody(),
                                                                 message.getGroupInfo(),
                                                                 message.getAttachments());

            Pair<Long, Long> messageAndThreadId =  database.insertSecureDecryptedMessageInbox(masterSecret, mediaMessage, -1);

            ApplicationContext.getInstance(context)
                      .getJobManager()
                      .add(new AttachmentDownloadJob(context, messageAndThreadId.first));

    if (smsMessageId.isPresent()) {
                DatabaseFactory.getSmsDatabase(context).deleteMessage(smsMessageId.get());
            }

            MessageNotifier.updateNotification(context, masterSecret.getMasterSecret().orNull(), messageAndThreadId.second);
        }

        private void handleSynchronizeSentMediaMessage(@NonNull MasterSecretUnion masterSecret,
                                                       @NonNull SentTranscriptMessage message,
                                                       @NonNull Optional<Long> smsMessageId)
      throws MmsException
        {
            MmsDatabase database     = DatabaseFactory.getMmsDatabase(context);
            Recipients recipients   = getSyncMessageDestination(message);
            OutgoingMediaMessage mediaMessage = new OutgoingMediaMessage(context, masterSecret, recipients,
                                                                  message.getMessage().getAttachments().get(),
                                                                  message.getMessage().getBody().orNull());

            mediaMessage = new OutgoingSecureMediaMessage(mediaMessage);

            long threadId = DatabaseFactory.getThreadDatabase(context).getThreadIdFor(recipients);
            long messageId = database.insertMessageOutbox(masterSecret, mediaMessage, threadId, false, message.getTimestamp());

            database.markAsSent(messageId, "push".getBytes(), 0);
            database.markAsPush(messageId);

            ApplicationContext.getInstance(context)
                              .getJobManager()
                              .add(new AttachmentDownloadJob(context, messageId));

            if (smsMessageId.isPresent())
            {
                DatabaseFactory.getSmsDatabase(context).deleteMessage(smsMessageId.get());
            }
            }
            */
        private void handleTextMessage(/*@NonNull MasterSecretUnion masterSecret,*/
                                       TextSecureEnvelope envelope,
                                       TextSecureDataMessage message/*,
                                           @NonNull Optional<Long> smsMessageId*/)
        {
            MessageDatabase database = DatabaseFactory.getMessageDatabase();
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
                                                                          (long)message.getTimestamp(), body,
                                                                          message.getGroupInfo());

                textMessage = new IncomingEncryptedMessage(textMessage, body);
                /*messageAndThreadId = */database.insertMessageInbox(/*masterSecret, */textMessage);
            /*}

            MessageNotifier.updateNotification(context, masterSecret.getMasterSecret().orNull(), messageAndThreadId.second);*/
        }
        /*
        private void handleSynchronizeSentTextMessage(@NonNull MasterSecretUnion masterSecret,
                                                      @NonNull SentTranscriptMessage message,
                                                      @NonNull Optional<Long> smsMessageId)
        {
            EncryptingSmsDatabase database = DatabaseFactory.getEncryptingSmsDatabase(context);
            Recipients recipients = getSyncMessageDestination(message);
            String body = message.getMessage().getBody().or("");
            OutgoingTextMessage outgoingTextMessage = new OutgoingTextMessage(recipients, body);

            long threadId = DatabaseFactory.getThreadDatabase(context).getThreadIdFor(recipients);
            long messageId = database.insertMessageOutbox(masterSecret, threadId, outgoingTextMessage, false, message.getTimestamp());

            database.markAsSent(messageId);
            database.markAsPush(messageId);
            database.markAsSecure(messageId);

            if (smsMessageId.isPresent())
            {
                database.deleteMessage(smsMessageId.get());
            }
        }

        private void handleInvalidVersionMessage(@NonNull MasterSecretUnion masterSecret,
                                                 @NonNull TextSecureEnvelope envelope,
                                                 @NonNull Optional<Long> smsMessageId)
        {
            EncryptingSmsDatabase smsDatabase = DatabaseFactory.getEncryptingSmsDatabase(context);

            if (!smsMessageId.isPresent())
            {
                Pair<Long, Long> messageAndThreadId = insertPlaceholder(envelope);
                smsDatabase.markAsInvalidVersionKeyExchange(messageAndThreadId.first);
                MessageNotifier.updateNotification(context, masterSecret.getMasterSecret().orNull(), messageAndThreadId.second);
            }
            else
            {
                smsDatabase.markAsInvalidVersionKeyExchange(smsMessageId.get());
            }
        }

        private void handleCorruptMessage(@NonNull MasterSecretUnion masterSecret,
                                          @NonNull TextSecureEnvelope envelope,
                                          @NonNull Optional<Long> smsMessageId)
        {
            EncryptingSmsDatabase smsDatabase = DatabaseFactory.getEncryptingSmsDatabase(context);

            if (!smsMessageId.isPresent())
            {
                Pair<Long, Long> messageAndThreadId = insertPlaceholder(envelope);
                smsDatabase.markAsDecryptFailed(messageAndThreadId.first);
                MessageNotifier.updateNotification(context, masterSecret.getMasterSecret().orNull(), messageAndThreadId.second);
            }
            else
            {
                smsDatabase.markAsDecryptFailed(smsMessageId.get());
            }
        }

        private void handleNoSessionMessage(@NonNull MasterSecretUnion masterSecret,
                                            @NonNull TextSecureEnvelope envelope,
                                            @NonNull Optional<Long> smsMessageId)
        {
            EncryptingSmsDatabase smsDatabase = DatabaseFactory.getEncryptingSmsDatabase(context);

            if (!smsMessageId.isPresent())
            {
                Pair<Long, Long> messageAndThreadId = insertPlaceholder(envelope);
                smsDatabase.markAsNoSession(messageAndThreadId.first);
                MessageNotifier.updateNotification(context, masterSecret.getMasterSecret().orNull(), messageAndThreadId.second);
            }
            else
            {
                smsDatabase.markAsNoSession(smsMessageId.get());
            }
        }

        private void handleLegacyMessage(@NonNull MasterSecretUnion masterSecret,
                                         @NonNull TextSecureEnvelope envelope,
                                         @NonNull Optional<Long> smsMessageId)
        {
            EncryptingSmsDatabase smsDatabase = DatabaseFactory.getEncryptingSmsDatabase(context);

            if (!smsMessageId.isPresent())
            {
                Pair<Long, Long> messageAndThreadId = insertPlaceholder(envelope);
                smsDatabase.markAsLegacyVersion(messageAndThreadId.first);
                MessageNotifier.updateNotification(context, masterSecret.getMasterSecret().orNull(), messageAndThreadId.second);
            }
            else
            {
                smsDatabase.markAsLegacyVersion(smsMessageId.get());
            }
        }

        private void handleDuplicateMessage(@NonNull MasterSecretUnion masterSecret,
                                            @NonNull TextSecureEnvelope envelope,
                                            @NonNull Optional<Long> smsMessageId)
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

        private void handleUntrustedIdentityMessage(@NonNull MasterSecretUnion masterSecret,
                                                    @NonNull TextSecureEnvelope envelope,
                                                    @NonNull Optional<Long> smsMessageId)
        {
            try
            {
                EncryptingSmsDatabase database = DatabaseFactory.getEncryptingSmsDatabase(context);
                Recipients recipients = RecipientFactory.getRecipientsFromString(context, envelope.getSource(), false);
                long recipientId = recipients.getPrimaryRecipient().getRecipientId();
                PreKeyWhisperMessage whisperMessage = new PreKeyWhisperMessage(envelope.getLegacyMessage());
                IdentityKey identityKey = whisperMessage.getIdentityKey();
                String encoded = Base64.encodeBytes(envelope.getLegacyMessage());
                IncomingTextMessage textMessage = new IncomingTextMessage(envelope.getSource(), envelope.getSourceDevice(),
                                                                               envelope.getTimestamp(), encoded,
                                                                               Optional.< TextSecureGroup > absent());

                if (!smsMessageId.isPresent())
                {
                    IncomingPreKeyBundleMessage bundleMessage = new IncomingPreKeyBundleMessage(textMessage, encoded);
                    Pair<Long, Long> messageAndThreadId = database.insertMessageInbox(masterSecret, bundleMessage);

                    database.setMismatchedIdentity(messageAndThreadId.first, recipientId, identityKey);
                    MessageNotifier.updateNotification(context, masterSecret.getMasterSecret().orNull(), messageAndThreadId.second);
                }
                else
                {
                    database.updateMessageBody(masterSecret, smsMessageId.get(), encoded);
                    database.markAsPreKeyBundle(smsMessageId.get());
                    database.setMismatchedIdentity(smsMessageId.get(), recipientId, identityKey);
                }
            }
            catch (InvalidMessageException | InvalidVersionException e) {
                throw new AssertionError(e);
            }
            }

  private Pair<Long, Long> insertPlaceholder(@NonNull TextSecureEnvelope envelope)
        {
            EncryptingSmsDatabase database = DatabaseFactory.getEncryptingSmsDatabase(context);
            IncomingTextMessage textMessage = new IncomingTextMessage(envelope.getSource(), envelope.getSourceDevice(),
                                                                        envelope.getTimestamp(), "",
                                                                        Optional.< TextSecureGroup > absent());

            textMessage = new IncomingEncryptedMessage(textMessage, "");
            return database.insertMessageInbox(textMessage);
        }

        private Recipients getSyncMessageDestination(SentTranscriptMessage message)
        {
            if (message.getMessage().getGroupInfo().isPresent())
            {
                return RecipientFactory.getRecipientsFromString(context, GroupUtil.getEncodedId(message.getMessage().getGroupInfo().get().getGroupId()), false);
            }
            else
            {
                return RecipientFactory.getRecipientsFromString(context, message.getDestination().get(), false);
            }
        }*/
    }

}