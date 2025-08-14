using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Planorama.User.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserPrivacySettingsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserCredentials_Id",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "UserPrivacySettings",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsPrivate = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPrivacySettings", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserPrivacySettings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPrivacySettings_UserId_IsPrivate",
                table: "UserPrivacySettings",
                columns: new[] { "UserId", "IsPrivate" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCredentials_Users_UserId",
                table: "UserCredentials",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCredentials_Users_UserId",
                table: "UserCredentials");

            migrationBuilder.DropTable(
                name: "UserPrivacySettings");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserCredentials_Id",
                table: "Users",
                column: "Id",
                principalTable: "UserCredentials",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
