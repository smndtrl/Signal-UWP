using libtextsecure;
using libtextsecure.push;
using Signal.Tasks.Library;
using Strilanc.Value;
using System;
using TextSecure;
using Signal.Push;
using Signal.Util;
using TextSecure.util;
using Signal.Database;

namespace Signal.Tasks
{
    public class PushSendTask : SendTask
    {
        public PushSendTask(string destination)
        {

        }

        public override void onAdded()
        {
            throw new NotImplementedException("SendTask onAdded");
        }

        public new void OnCanceled()
        {
            throw new NotImplementedException("SendTask OnCanceled");
        }

        protected override string Execute()
        {
            throw new NotImplementedException("SendTask Execure");
        }

        protected TextSecureAddress getPushAddress(String number)
        {
            String e164number = Utils.canonicalizeNumber(number);
            String relay = DatabaseFactory.getDirectoryDatabase().getRelay(e164number);
            return new TextSecureAddress(e164number, relay == null ? May<string>.NoValue : new May<string>(relay));
        }

        //protected TextSecureMessageSender messageSender = new TextSecureMessageSender(TextSecureCommunicationFactory.PUSH_URL, new TextSecurePushTrustStore(), TextSecurePreferences.getLocalNumber(), TextSecurePreferences.getPushServerPassword(), new TextSecureAxolotlStore(),
        //                                                                           May<TextSecureMessageSender.EventListener>.NoValue, App.CurrentVersion);

        /*
                protected List<TextSecureAttachment> getAttachmentsFor(MasterSecret masterSecret, List<Attachment> parts)
                {
                    List<TextSecureAttachment> attachments = new LinkedList<>();

                    for (final Attachment attachment : parts)
                    {
                        if (ContentType.isImageType(attachment.getContentType()) ||
                            ContentType.isAudioType(attachment.getContentType()) ||
                            ContentType.isVideoType(attachment.getContentType()))
                        {
                            try
                            {
                                if (attachment.getDataUri() == null) throw new IOException("Assertion failed, outgoing attachment has no data!");
                                InputStream is = PartAuthority.getAttachmentStream(context, masterSecret, attachment.getDataUri());
                                attachments.add(TextSecureAttachment.newStreamBuilder()
                                                                    .withStream(is)
                                                                    .withContentType(attachment.getContentType())
                                                                    .withLength(attachment.getSize())
                                                                    .withListener(new ProgressListener() {
                                                        @Override
                                                                      public void onAttachmentProgress(long total, long progress)
                {
                    EventBus.getDefault().postSticky(new PartProgressEvent(attachment, total, progress));
                }
            })
                                                      .build());
                } catch (IOException ioe) {
                  Log.w(TAG, "Couldn't open attachment", ioe);
                }
              }
            }

            return attachments;
          }

          protected void notifyMediaMessageDeliveryFailed(Context context, long messageId)
        {
            long threadId = DatabaseFactory.getMmsDatabase(context).getThreadIdForMessage(messageId);
            Recipients recipients = DatabaseFactory.getThreadDatabase(context).getRecipientsForThreadId(threadId);

            if (threadId != -1 && recipients != null)
            {
                MessageNotifier.notifyMessageDeliveryFailed(context, recipients, threadId);
            }
        }*/
    }
}
