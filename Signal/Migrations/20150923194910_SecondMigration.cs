using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Migrations.Builders;
using Microsoft.Data.Entity.Migrations.Operations;

namespace SignalMigrations
{
    public partial class SecondMigration : Migration
    {
        public override void Up(MigrationBuilder migration)
        {
            migration.CreateTable(
                name: "Threads",
                columns: table => new
                {
                    ThreadId = table.Column(type: "INTEGER", nullable: false),
                        //.Annotation("Sqlite:Autoincrement", true),
                    Body = table.Column(type: "TEXT", nullable: true),
                    Count = table.Column(type: "INTEGER", nullable: false),
                    Date = table.Column(type: "TEXT", nullable: false),
                    Read = table.Column(type: "INTEGER", nullable: false),
                    Snippet = table.Column(type: "TEXT", nullable: true),
                    SnippetType = table.Column(type: "INTEGER", nullable: false),
                    Type = table.Column(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Thread", x => x.ThreadId);
                });
            migration.CreateTable(
                name: "Recipient",
                columns: table => new
                {
                    RecipientId = table.Column(type: "INTEGER", nullable: false),
                        //.Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column(type: "TEXT", nullable: true),
                    Number = table.Column(type: "TEXT", nullable: true),
                    ThreadThreadId = table.Column(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipient", x => x.RecipientId);
                    table.ForeignKey(
                        name: "FK_Recipient_Thread_ThreadThreadId",
                        columns: x => x.ThreadThreadId,
                        referencedTable: "Threads",
                        referencedColumn: "ThreadId");
                });
            migration.RenameTable(
                name: "Contact",
                newName: "Contacts");
        }

        public override void Down(MigrationBuilder migration)
        {
            migration.DropTable("Recipient");
            migration.DropTable("Threads");
            migration.RenameTable(
                name: "Contacts",
                newName: "Contact");
        }
    }
}
