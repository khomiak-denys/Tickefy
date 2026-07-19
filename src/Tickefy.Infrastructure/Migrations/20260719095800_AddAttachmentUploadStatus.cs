using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tickefy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAttachmentUploadStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Attachments",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Attachments");
        }
    }
}
