using Signal.Database;
using Signal.Database.interfaces;
using Signal.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.database;
using TextSecure.recipient;
using TextSecure.util;

namespace Signal.database
{
    public interface IDataService
    {
        Task<IEnumerable<Contact>> getContacts();
        Task<IEnumerable<Thread>> getThreads();
        void CreateThread(Thread thread);
        Task<IEnumerable<Contact>> getDictionary();
        Task<IEnumerable<Message>> getMessages(long threadId);
    }

    public class Design : IDataService
    {
        IList<Contact> Contacts;
        IList<Thread> Threads;
        IList<Message> Messages;

        public Design()
        {
            Contacts = new List<Contact>();
            Threads = new List<Thread>();
            Messages = new List<Message>();
            for (var i = 0; i <= 100; i++)
            {
                if (i < 7) Contacts.Add(new Contact() { name = "Simon" + i, number = "+491122330" + i, label = "mobile" });
                if (i < 2) Threads.Add(new Thread() { Body = "Last message", Read = (i % 2) == 1, Count = i, Date = DateTime.Now.AddSeconds(-60 * i), Recipients = new List<Recipient>() { new Recipient("Recip " + i, "+49001122" + i, i) }, ThreadId = i % 2 });
                //                     var msg = new Message() { MessageId = i, ThreadId = i % 2, Body = "Message " + i, IsFailed = i % 5 == 0, IsOutgoing = i % 3 == 0, Date = DateTime.Now.AddSeconds(-15 * i) ;

                if (i < 50)
                {
                    var msg = new Message(i, "Message " + i, null, new Recipient("Name" + i, "+49112233" + i, i), 1, DateTime.Now.AddMinutes(-60 * i), DateTime.Now.AddSeconds(-15 * i), i % 2, 1, 1, 1);
                    Messages.Add(msg);
                }
            }
        }

        public Task<IEnumerable<Contact>> getContacts()
        {
            return Task.FromResult<IEnumerable<Contact>>(Contacts);
        }

        public Task<IEnumerable<Thread>> getThreads()
        {
            return Task.FromResult<IEnumerable<Thread>>(Threads);
        }

        public Task<IEnumerable<Message>> getMessages(long threadId)
        {
            return Task.FromResult<IEnumerable<Message>>(Messages.Where(p => p.ThreadId == threadId));
        }

        public Task<IEnumerable<Contact>> getDictionary()
        {
            return Task.FromResult<IEnumerable<Contact>>(Contacts);
        }

        public void CreateThread(Thread thread)
        {
            Threads.Add(thread);
        }
    }

    public class Real : IDataService
    {
        private SignalContext context;

        private Database.EF.ThreadDatabase Threads;

        public Real()
        {

            context = new SignalContext();
            Threads = new Database.EF.ThreadDatabase(context);
        }

        public async Task<IEnumerable<Contact>> getContacts()
        {
            return await new ContactsDatabase().getContacts();
        }

        public Task<IEnumerable<Contact>> getDictionary()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Message>> getMessages(long threadId)
        {
            return await DatabaseFactory.getTextMessageDatabase().getMessages(threadId);
        }

        public async Task<IEnumerable<Thread>> getThreads()
        {
            return context.Threads.Where(t => true).ToList();
            //return await DatabaseFactory.getThreadDatabase().getThreads();
        }

        public void CreateThread(Thread thread)
        {
            throw new NotImplementedException();
        }
    }
}
