using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jobino.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OwnerUserId",
                table: "Companies",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_OwnerUserId",
                table: "Companies",
                column: "OwnerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Users_OwnerUserId",
                table: "Companies",
                column: "OwnerUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Users_OwnerUserId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_OwnerUserId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "OwnerUserId",
                table: "Companies");
        }
    }
}
