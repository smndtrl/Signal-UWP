using Signal.Database;
using Signal.Database.interfaces;
using Signal.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.recipient;
using TextSecure.util;

namespace Signal.Database.EF
{
    public class ThreadDatabase : ThreadDatabaseHelper, IThreadDatabase
    {

        SignalContext context;

        public ThreadDatabase(SignalContext context)
        {
            this.context = context;
        }

        public async Task UpdateThread(long threadId, long count, string body, long date, long type)
        {
            var thread = context.Threads.Where(t => t.ThreadId == threadId).First();

            thread.Count = count;
            thread.Body = body;
            thread.Date = Util.GetDateTime((double) date);
            thread.Type = type;

            await context.SaveChangesAsync();
        }
        public async Task UpdateSnippet(long threadId, string snippet, long date, long type)
        {
            var thread = context.Threads.Where(t => t.ThreadId == threadId).First();

            thread.Snippet = snippet;
            thread.Date = Util.GetDateTime((double)date);
            thread.Type = type;

            await context.SaveChangesAsync();
        }

        public async Task SetRead(long threadId)
        {
            var thread = context.Threads.Where(t => t.ThreadId == threadId).First();
            thread.Read = true;
            await context.SaveChangesAsync();
        }

        public async Task SetAllThreadsRead()
        {
            var threads = context.Threads.Where(t => true).ToList();
            threads.ForEach(thread =>
            {
                thread.Read = true;
            });
            
            await context.SaveChangesAsync();
        }

        public async Task SetUnread(long threadId)
        {
            var thread = context.Threads.Where(t => t.ThreadId == threadId).First();
            thread.Read = false;
            await context.SaveChangesAsync();
        }

        public Task<long> GetThreadIdFor(Recipients recipients)
        {
            throw new NotImplementedException();
        }

        public async Task<long> GetThreadIdFor(Recipients recipients, int distributionType)
        {
            long[] recipientIds = getRecipientIds(recipients);
            string recipientsList = getRecipientsAsString(recipientIds);


            try
            {
                var query = context.Threads.Where(t => t.RecipientIds == recipientsList);// TODO: recipient refactor
                var first = query.Count() == 0 ? null : query.First();

                if (query != null && first != null)
                    return first.ThreadId;
                else
                    return await CreateThreadForRecipients(recipients, recipientIds.Length, distributionType);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {

            }
        }

        private async Task<long> CreateThreadForRecipients(Recipients recipients, int recipientCount, int distributionType)
        {
            long[] recipientIds = getRecipientIds(recipients);
            string recipientsList = getRecipientsAsString(recipientIds);

            var thread = new Thread()
            {
                Date = DateTime.Now,
                RecipientIds = recipientsList,
                //Recipients = recipients.getRecipientsList(),
                Count = 0
            };

            if (recipientCount > 1)
            {
                thread.Type = distributionType;
            }

            context.Add(thread);
            await context.SaveChangesAsync();

            return thread.ThreadId;
        }

        public async Task DeleteThread(long threadId)
        {
            var thread = context.Threads.Where(t => t.ThreadId == threadId).First();
            context.Threads.Remove(thread);

            await context.SaveChangesAsync();
        }

        public async Task DeleteThreads(ICollection<long> threadIds)
        {
            var threads = context.Threads.Where(t => threadIds.Contains(t.ThreadId));
            context.Threads.RemoveRange(threads);

            await context.SaveChangesAsync();
        }

        public async Task DeleteAllThreads()
        {
            var threads = context.Threads.Where(t => true);
            context.Threads.RemoveRange(threads);

            await context.SaveChangesAsync();
        }
    }
}
