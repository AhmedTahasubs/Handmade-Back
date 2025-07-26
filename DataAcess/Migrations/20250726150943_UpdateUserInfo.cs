using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAcess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql("UPDATE AspNetUsers SET HasWhatsApp = 0 WHERE HasWhatsApp IS NULL");

			// ثانيًا: نعدل نوع العمود إلى not null + default = false
			migrationBuilder.AlterColumn<bool>(
				name: "HasWhatsApp",
				table: "AspNetUsers",
				type: "bit",
				nullable: false,
				defaultValue: false,
				oldClrType: typeof(bool),
				oldType: "bit",
				oldNullable: true);
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.AlterColumn<bool>(
			name: "HasWhatsApp",
			table: "AspNetUsers",
			type: "bit",
			nullable: true,
			oldClrType: typeof(bool),
			oldType: "bit",
			oldNullable: false);
		}
    }
}
