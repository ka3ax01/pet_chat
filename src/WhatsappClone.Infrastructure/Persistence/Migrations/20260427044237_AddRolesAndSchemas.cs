using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WhatsappClone.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesAndSchemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageReads_Users_UserId",
                table: "MessageReads");

            migrationBuilder.DropIndex(
                name: "IX_MessageReads_MessageId",
                table: "MessageReads");

            migrationBuilder.EnsureSchema(
                name: "audit");

            migrationBuilder.EnsureSchema(
                name: "messaging");

            migrationBuilder.EnsureSchema(
                name: "auth");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Users",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "RefreshSessions",
                newName: "RefreshSessions",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "Messages",
                newName: "Messages",
                newSchema: "messaging");

            migrationBuilder.RenameTable(
                name: "MessageReads",
                newName: "MessageReads",
                newSchema: "messaging");

            migrationBuilder.RenameTable(
                name: "MessageAttachments",
                newName: "MessageAttachments",
                newSchema: "messaging");

            migrationBuilder.RenameTable(
                name: "EntityActions",
                newName: "EntityActions",
                newSchema: "audit");

            migrationBuilder.RenameTable(
                name: "Chats",
                newName: "Chats",
                newSchema: "messaging");

            migrationBuilder.RenameTable(
                name: "ChatMembers",
                newName: "ChatMembers",
                newSchema: "messaging");

            migrationBuilder.RenameTable(
                name: "BackendLogs",
                newName: "BackendLogs",
                newSchema: "audit");

            migrationBuilder.AlterColumn<string>(
                name: "RefreshTokenHash",
                schema: "auth",
                table: "RefreshSessions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "auth",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "auth",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "auth",
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "User" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshSessions_ExpiresAt",
                schema: "auth",
                table: "RefreshSessions",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshSessions_RefreshTokenHash",
                schema: "auth",
                table: "RefreshSessions",
                column: "RefreshTokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageReads_MessageId_UserId",
                schema: "messaging",
                table: "MessageReads",
                columns: new[] { "MessageId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                schema: "auth",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                schema: "auth",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageReads_Users_UserId",
                schema: "messaging",
                table: "MessageReads",
                column: "UserId",
                principalSchema: "auth",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageReads_Users_UserId",
                schema: "messaging",
                table: "MessageReads");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "auth");

            migrationBuilder.DropIndex(
                name: "IX_RefreshSessions_ExpiresAt",
                schema: "auth",
                table: "RefreshSessions");

            migrationBuilder.DropIndex(
                name: "IX_RefreshSessions_RefreshTokenHash",
                schema: "auth",
                table: "RefreshSessions");

            migrationBuilder.DropIndex(
                name: "IX_MessageReads_MessageId_UserId",
                schema: "messaging",
                table: "MessageReads");

            migrationBuilder.RenameTable(
                name: "Users",
                schema: "auth",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "RefreshSessions",
                schema: "auth",
                newName: "RefreshSessions");

            migrationBuilder.RenameTable(
                name: "Messages",
                schema: "messaging",
                newName: "Messages");

            migrationBuilder.RenameTable(
                name: "MessageReads",
                schema: "messaging",
                newName: "MessageReads");

            migrationBuilder.RenameTable(
                name: "MessageAttachments",
                schema: "messaging",
                newName: "MessageAttachments");

            migrationBuilder.RenameTable(
                name: "EntityActions",
                schema: "audit",
                newName: "EntityActions");

            migrationBuilder.RenameTable(
                name: "Chats",
                schema: "messaging",
                newName: "Chats");

            migrationBuilder.RenameTable(
                name: "ChatMembers",
                schema: "messaging",
                newName: "ChatMembers");

            migrationBuilder.RenameTable(
                name: "BackendLogs",
                schema: "audit",
                newName: "BackendLogs");

            migrationBuilder.AlterColumn<string>(
                name: "RefreshTokenHash",
                table: "RefreshSessions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.CreateIndex(
                name: "IX_MessageReads_MessageId",
                table: "MessageReads",
                column: "MessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageReads_Users_UserId",
                table: "MessageReads",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
