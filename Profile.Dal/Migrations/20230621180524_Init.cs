using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Profile.Dal.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cap");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "outbox",
                schema: "cap",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageType = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    ActivityId = table.Column<string>(type: "text", nullable: true),
                    Retries = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    Error = table.Column<string>(type: "text", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    LockTimeout = table.Column<TimeSpan>(type: "interval", nullable: false, defaultValue: new TimeSpan(0, 0, 0, 30, 0), comment: "Maximum allowable blocking time"),
                    LockId = table.Column<Guid>(type: "uuid", nullable: true, comment: "Idempotency key (unique key of the thread that captured the lock)"),
                    LockExpirationTimeUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "Preventive timeout (maximum lifetime of actuality 'LockId')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outbox", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    EmailConfirmed = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    MiddleName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_outbox_CorrelationId",
                schema: "cap",
                table: "outbox",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_outbox_CreatedUtc",
                schema: "cap",
                table: "outbox",
                column: "CreatedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_outbox_Retries",
                schema: "cap",
                table: "outbox",
                column: "Retries");

            migrationBuilder.CreateIndex(
                name: "IX_outbox_Status",
                schema: "cap",
                table: "outbox",
                column: "Status",
                filter: "\"Status\" in (0,1)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "outbox",
                schema: "cap");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
