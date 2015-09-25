using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Migrations.Builders;
using Microsoft.Data.Entity.Migrations.Operations;

namespace SignalMigrations
{
    public partial class FirstMigration : Migration
    {
        public override void Up(MigrationBuilder migration)
        {
            migration.CreateTable(
                name: "Contact",
                columns: table => new
                {
                    name = table.Column(type: "TEXT", nullable: false),
                    label = table.Column(type: "TEXT", nullable: true),
                    number = table.Column(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contact", x => x.name);
                });
        }

        public override void Down(MigrationBuilder migration)
        {
            migration.DropTable("Contact");
        }
    }
}
