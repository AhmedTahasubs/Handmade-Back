using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAcess.Migrations
{
    /// <inheritdoc />
    public partial class AddIdCardImageIdCloumnToApplicationUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdCardImageId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_IdCardImageId",
                table: "AspNetUsers",
                column: "IdCardImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Images_IdCardImageId",
                table: "AspNetUsers",
                column: "IdCardImageId",
                principalTable: "Images",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Images_IdCardImageId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_IdCardImageId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IdCardImageId",
                table: "AspNetUsers");
        }
    }
}
