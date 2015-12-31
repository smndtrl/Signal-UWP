using GalaSoft.MvvmLight.Messaging;
using Signal.Database;
using Signal.Database.interfaces;
using Signal.Models;
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
        //Task<IEnumerable<Contact>> getContacts();
        Task<IEnumerable<Thread>> getThreads();
        void CreateThread(Thread thread);
        Task<IEnumerable<TextSecureDirectory.Directory>> getDictionary();
        Task<IEnumerable<Message>> getMessages(long threadId);
        long getMessagesCount(long threadId);
    }

    public class Design : IDataService
    {
        IList<Contact> Contacts;
        IList<Thread> Threads;
        IList<Message> Messages;
        IList<TextSecureDirectory.Directory> Directory;

        public Design()
        {
            Contacts = new List<Contact>();
            Threads = new List<Thread>();
            Messages = new List<Message>();
            Directory = new List<TextSecureDirectory.Directory>();


            for (var i = 0; i <= 100; i++)
            {
                if (i < 7) Contacts.Add(new Contact() { name = "Simon" + i, number = "+491122330" + i, label = "mobile" });
                if (i < 7) Directory.Add(new TextSecureDirectory.Directory() {Name = "Name " + i, ContactId = ""+i, DirectoryId = i, Number = "01234"+i, Registered =  i% 2, Relay = "", Time = DateTime.Now });

                if (i < 5) Threads.Add(new Thread() { Body = "Last message", Read = (i % 2) == 1, Count = i, Date = DateTime.Now.AddSeconds(-60 * i), Recipients = new Recipients(new List<Recipient>() { new Recipient("Recip " + i, "+49001122" + i, i, null) }, null), ThreadId = i % 2 });
                //                     var msg = new Message() { MessageId = i, ThreadId = i % 2, Body = "Message " + i, IsFailed = i % 5 == 0, IsOutgoing = i % 3 == 0, Date = DateTime.Now.AddSeconds(-15 * i) ;

                if (i < 50)
                {
                    var msg = new Message()
                    {
                        MessageId = i,
                        Body = "Message " + i,
                        Address = "+49112233" + i,
                        AddressDeviceId = 1,
                        DateSent = DateTime.Now.AddMinutes(-60 * i),
                        DateReceived = DateTime.Now.AddSeconds(-15 * i),
                        Read = i % 2 == 1 ? true : false,
                        ReceiptCount = 1,
                        Type = 1,
                        ThreadId = i % 5
                    };

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

        public Task<IEnumerable<TextSecureDirectory.Directory>> getDictionary()
        {
            return Task.FromResult<IEnumerable<TextSecureDirectory.Directory>>(Directory);
        }

        public void CreateThread(Thread thread)
        {
            Threads.Add(thread);
        }

        public long getMessagesCount(long threadId)
        {
            return (Messages.Where(p => p.ThreadId == threadId)).Count();
        }
    }

    public class Sqlite : IDataService
    {
       // private SignalContext context;

        //private Database.EF.ThreadDatabase Threads;

        public Sqlite()
        {

            //context = new SignalContext();
            //Threads = new Database.EF.ThreadDatabase(context);
        }

        /*public async Task<IEnumerable<Contact>> getContacts()
        {
            return await DatabaseFactory.getContactsDatabase().getContacts();
        }*/

        public async Task<IEnumerable<TextSecureDirectory.Directory>> getDictionary()
        {
            return await DatabaseFactory.getDirectoryDatabase().GetAllAsync();
        }

        public async Task<IEnumerable<Message>> getMessages(long threadId)
        {
            return await DatabaseFactory.getTextMessageDatabase().getMessages(threadId);
        }

        public async Task<IEnumerable<Thread>> getThreads()
        {
            //return context.Threads.Where(t => true).ToList();
            return await DatabaseFactory.getThreadDatabase().GetAllAsync();
        }

        public void CreateThread(Thread thread)
        {
            throw new NotImplementedException();
        }

        public long getMessagesCount(long threadId)
        {
            return DatabaseFactory.getTextMessageDatabase().getMessagesCount(threadId);
        }
    }
}
