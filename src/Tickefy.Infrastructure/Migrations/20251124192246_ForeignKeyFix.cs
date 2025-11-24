using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tickefy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ForeignKeyFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Tickets_TicketId1",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_TicketId1",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "TicketId1",
                table: "Comments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TicketId1",
                table: "Comments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Comments_TicketId1",
                table: "Comments",
                column: "TicketId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Tickets_TicketId1",
                table: "Comments",
                column: "TicketId1",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
