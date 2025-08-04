using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAcess.Migrations
{
    /// <inheritdoc />
    public partial class imageCusom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "CustomerRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerRequests_ImageId",
                table: "CustomerRequests",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerRequests_Images_ImageId",
                table: "CustomerRequests",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerRequests_Images_ImageId",
                table: "CustomerRequests");

            migrationBuilder.DropIndex(
                name: "IX_CustomerRequests_ImageId",
                table: "CustomerRequests");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "CustomerRequests");
        }
    }
}
