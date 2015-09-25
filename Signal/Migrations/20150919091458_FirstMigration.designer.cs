using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations.Infrastructure;
using Signal.Database;

namespace SignalMigrations
{
    [ContextType(typeof(SignalContext))]
    partial class FirstMigration
    {
        public override string Id
        {
            get { return "20150919091458_FirstMigration"; }
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
                });
        }
    }
}
