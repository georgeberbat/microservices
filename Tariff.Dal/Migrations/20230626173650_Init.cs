using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tariff.Dal.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cap");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:permission_mode", "read,write")
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
                name: "route",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_route", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tariff",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tariff", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "route_unit",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    route_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tariff_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_route_unit", x => x.id);
                    table.ForeignKey(
                        name: "FK_RouteUnit_Route",
                        column: x => x.route_id,
                        principalTable: "route",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RouteUnit_Tariff",
                        column: x => x.tariff_id,
                        principalTable: "tariff",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tariff_unit",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tariff_id = table.Column<Guid>(type: "uuid", nullable: false),
                    location_id = table.Column<Guid>(type: "uuid", nullable: false),
                    next_location_id = table.Column<Guid>(type: "uuid", nullable: false),
                    weight_scale_coefficient = table.Column<double>(type: "double precision", nullable: false),
                    distance = table.Column<int>(type: "integer", nullable: false),
                    created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tariff_unit", x => x.id);
                    table.ForeignKey(
                        name: "FK_TariffUnit_Tariff",
                        column: x => x.tariff_id,
                        principalTable: "tariff",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_route_unit_route_id",
                table: "route_unit",
                column: "route_id");

            migrationBuilder.CreateIndex(
                name: "IX_route_unit_tariff_id",
                table: "route_unit",
                column: "tariff_id");

            migrationBuilder.CreateIndex(
                name: "IX_tariff_unit_tariff_id",
                table: "tariff_unit",
                column: "tariff_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "outbox",
                schema: "cap");

            migrationBuilder.DropTable(
                name: "route_unit");

            migrationBuilder.DropTable(
                name: "tariff_unit");

            migrationBuilder.DropTable(
                name: "route");

            migrationBuilder.DropTable(
                name: "tariff");
        }
    }
}
