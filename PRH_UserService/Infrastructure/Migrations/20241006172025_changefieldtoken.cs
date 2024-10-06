using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changefieldtoken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefreshToken",
                table: "Tokens",
                newName: "RefreshTokenHash");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Tokens",
                newName: "IsUsed");

            migrationBuilder.RenameIndex(
                name: "IX_Tokens_RefreshToken",
                table: "Tokens",
                newName: "IX_Tokens_RefreshTokenHash");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Tokens",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Tokens");

            migrationBuilder.RenameColumn(
                name: "RefreshTokenHash",
                table: "Tokens",
                newName: "RefreshToken");

            migrationBuilder.RenameColumn(
                name: "IsUsed",
                table: "Tokens",
                newName: "IsActive");

            migrationBuilder.RenameIndex(
                name: "IX_Tokens_RefreshTokenHash",
                table: "Tokens",
                newName: "IX_Tokens_RefreshToken");
        }
    }
}
