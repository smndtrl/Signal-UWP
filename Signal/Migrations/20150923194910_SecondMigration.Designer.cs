using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations.Infrastructure;
using Signal.Database;

namespace SignalMigrations
{
    [ContextType(typeof(SignalContext))]
    partial class SecondMigration
    {
        public override string Id
        {
            get { return "20150923194910_SecondMigration"; }
        }

        public override string ProductVersion
        {
            get { return "7.0.0-beta6-13815"; }
        }

        public override void BuildTargetModel(ModelBuilder builder)
        {
            builder
                .Annotation("ProductVersion", "7.0.0-beta6-13815");

            builder.Entity("Signal.Model.Contact", b =>
                {
                    b.Property<string>("name");

                    b.Property<string>("label");

                    b.Property<string>("number");

                    b.Key("name");

                    b.Annotation("Relational:TableName", "Contacts");
                });

            builder.Entity("Signal.Model.Thread", b =>
                {
                    b.Property<long>("ThreadId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Body");

                    b.Property<long>("Count");

                    b.Property<DateTime>("Date");

                    b.Property<bool>("Read");

                    b.Property<string>("Snippet");

                    b.Property<long>("SnippetType");

                    b.Property<long>("Type");

                    b.Key("ThreadId");

                    b.Annotation("Relational:TableName", "Threads");
                });

            builder.Entity("TextSecure.recipient.Recipient", b =>
                {
                    b.Property<long>("RecipientId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("Number");

                    b.Property<long?>("ThreadThreadId");

                    b.Key("RecipientId");
                });

            builder.Entity("TextSecure.recipient.Recipient", b =>
                {
                    b.Reference("Signal.Model.Thread")
                        .InverseCollection()
                        .ForeignKey("ThreadThreadId");
                });
        }
    }
}
