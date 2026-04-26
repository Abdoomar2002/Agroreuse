using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agroreuse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFcmDeviceToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FcmDeviceToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FcmDeviceToken",
                table: "AspNetUsers");
        }
    }
}
