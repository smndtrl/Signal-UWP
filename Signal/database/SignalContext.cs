using Microsoft.Data.Entity;
using Signal.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TextSecure.recipient;
using Windows.Storage;

namespace Signal.Database
{
    public class SignalContext : DbContext
    {
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Thread> Threads { get; set; }
        public DbSet<Recipient> Recipients { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            SetDirectory(2, Windows.Storage.ApplicationData.Current.TemporaryFolder.Path);
            string filePath = Path.Combine(@"Filename=C:\Users\simon\AppData\Local\Packages\39705SimonDieterle.TextSecure_d662aag152hcy\LocalState\", "Signal.db");
            optionsBuilder.UseSqlite($"Data source={filePath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Thread>()
                .Property(t => t.ThreadId).Required();
        }
        [DllImport("sqlite3", EntryPoint = "sqlite3_win32_set_directory", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        static extern int SetDirectory(uint directoryType, string directoryPath);
    }
}

    
