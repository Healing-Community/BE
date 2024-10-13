#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations;

/// <inheritdoc />
public partial class changefieldtoken : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            "RefreshToken",
            "Tokens",
            "RefreshTokenHash");

        migrationBuilder.RenameColumn(
            "IsActive",
            "Tokens",
            "IsUsed");

        migrationBuilder.RenameIndex(
            "IX_Tokens_RefreshToken",
            table: "Tokens",
            newName: "IX_Tokens_RefreshTokenHash");

        migrationBuilder.AddColumn<int>(
            "Status",
            "Tokens",
            "integer",
            nullable: false,
            defaultValue: 0);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            "Status",
            "Tokens");

        migrationBuilder.RenameColumn(
            "RefreshTokenHash",
            "Tokens",
            "RefreshToken");

        migrationBuilder.RenameColumn(
            "IsUsed",
            "Tokens",
            "IsActive");

        migrationBuilder.RenameIndex(
            "IX_Tokens_RefreshTokenHash",
            table: "Tokens",
            newName: "IX_Tokens_RefreshToken");
    }
}