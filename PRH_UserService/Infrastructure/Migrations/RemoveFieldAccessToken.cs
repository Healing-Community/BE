#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations;

/// <inheritdoc />
public partial class RemoveFieldAccessToken : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            "IX_Tokens_AccessToken",
            "Tokens");

        migrationBuilder.DropColumn(
            "AccessToken",
            "Tokens");

        migrationBuilder.DropColumn(
            "RefreshTokenExpiresAt",
            "Tokens");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            "AccessToken",
            "Tokens",
            "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<DateTime>(
            "RefreshTokenExpiresAt",
            "Tokens",
            "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.CreateIndex(
            "IX_Tokens_AccessToken",
            "Tokens",
            "AccessToken",
            unique: true);
    }
}