using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fix_RefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefreshTokenHash",
                table: "Tokens",
                newName: "RefreshToken");

            migrationBuilder.RenameIndex(
                name: "IX_Tokens_RefreshTokenHash",
                table: "Tokens",
                newName: "IX_Tokens_RefreshToken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefreshToken",
                table: "Tokens",
                newName: "RefreshTokenHash");

            migrationBuilder.RenameIndex(
                name: "IX_Tokens_RefreshToken",
                table: "Tokens",
                newName: "IX_Tokens_RefreshTokenHash");
        }
    }
}
