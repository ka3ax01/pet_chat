using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhatsappClone.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBackendLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BackendLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Level = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Exception = table.Column<string>(type: "text", nullable: true),
                    Source = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    RequestPath = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    HttpMethod = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    StatusCode = table.Column<int>(type: "integer", nullable: true),
                    ElapsedMilliseconds = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    TraceId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PropertiesJson = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackendLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BackendLogs_Level",
                table: "BackendLogs",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_BackendLogs_Timestamp",
                table: "BackendLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_BackendLogs_TraceId",
                table: "BackendLogs",
                column: "TraceId");

            migrationBuilder.CreateIndex(
                name: "IX_BackendLogs_UserId",
                table: "BackendLogs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BackendLogs");
        }
    }
}
