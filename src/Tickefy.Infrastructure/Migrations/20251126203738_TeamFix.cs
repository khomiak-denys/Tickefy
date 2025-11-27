using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tickefy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TeamFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ManagerId",
                table: "Teams",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Teams_ManagerId",
                table: "Teams",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Users_ManagerId",
                table: "Teams",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Users_ManagerId",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_ManagerId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Teams");
        }
    }
}
