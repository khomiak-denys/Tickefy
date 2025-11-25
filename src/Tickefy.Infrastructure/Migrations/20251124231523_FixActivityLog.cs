using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tickefy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixActivityLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Tickets_TicketId1",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_TicketId1",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "TicketId1",
                table: "Attachments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TicketId1",
                table: "Attachments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_TicketId1",
                table: "Attachments",
                column: "TicketId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Tickets_TicketId1",
                table: "Attachments",
                column: "TicketId1",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
