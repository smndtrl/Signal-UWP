using Signal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.recipient;

namespace Signal.Database.interfaces
{
    public interface IThreadDatabase
    {
        //Task UpdateThread(long threadId, long count, string body, long date, long type);
        Task UpdateSnippet(long threadId, string snippet, long date, long type);

        Task SetRead(long threadId);
        Task SetUnread(long threadId);
        Task SetAllThreadsRead();

        Task<long> GetThreadIdFor(Recipients recipients);
        Task<long> GetThreadIdFor(Recipients recipients, int distributionType);

        Task DeleteThread(long threadId);
        Task DeleteThreads(ICollection<long> threadIds);
        Task DeleteAllThreads();
    }

    public abstract class ThreadDatabaseHelper {

        public static long[] getRecipientIds(Recipients recipients)
        {
            HashSet<long> recipientSet = new HashSet<long>();
            List<Recipient> recipientList = recipients.getRecipientsList();

            foreach (Recipient recipient in recipientList)
            {
                recipientSet.Add(recipient.getRecipientId());
            }

            long[] recipientArray = new long[recipientSet.Count];
            int i = 0;

            foreach (long recipientId in recipientSet)
            {
                recipientArray[i++] = recipientId;
            }

            Array.Sort(recipientArray);

            return recipientArray;
        }

        public static String getRecipientsAsString(long[] recipientIds)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < recipientIds.Length; i++)
            {
                if (i != 0) sb.Append(' ');
                sb.Append(recipientIds[i]);
            }

            return sb.ToString();
        }
    }
}
