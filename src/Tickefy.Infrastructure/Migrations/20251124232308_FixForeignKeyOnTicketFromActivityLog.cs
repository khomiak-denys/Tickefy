using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tickefy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixForeignKeyOnTicketFromActivityLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLogs_Tickets_TicketId1",
                table: "ActivityLogs");

            migrationBuilder.DropIndex(
                name: "IX_ActivityLogs_TicketId1",
                table: "ActivityLogs");

            migrationBuilder.DropColumn(
                name: "TicketId1",
                table: "ActivityLogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TicketId1",
                table: "ActivityLogs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_TicketId1",
                table: "ActivityLogs",
                column: "TicketId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLogs_Tickets_TicketId1",
                table: "ActivityLogs",
                column: "TicketId1",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
