#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations;

/// <inheritdoc />
public partial class token : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            "Updated",
            "Users",
            "UpdatedAt");

        migrationBuilder.RenameColumn(
            "Created",
            "Users",
            "CreatedAt");

        migrationBuilder.CreateTable(
            "Tokens",
            table => new
            {
                TokenId = table.Column<Guid>("uuid", nullable: false),
                UserId = table.Column<Guid>("uuid", nullable: false),
                AccessToken = table.Column<string>("text", nullable: false),
                RefreshToken = table.Column<string>("text", nullable: false),
                IssuedAt = table.Column<DateTime>("timestamp with time zone", nullable: false),
                ExpiresAt = table.Column<DateTime>("timestamp with time zone", nullable: false),
                RefreshTokenExpiresAt = table.Column<DateTime>("timestamp with time zone", nullable: false),
                IsActive = table.Column<bool>("boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Tokens", x => x.TokenId);
                table.ForeignKey(
                    "FK_Tokens_Users_UserId",
                    x => x.UserId,
                    "Users",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            "IX_Tokens_AccessToken",
            "Tokens",
            "AccessToken",
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_Tokens_RefreshToken",
            "Tokens",
            "RefreshToken",
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_Tokens_UserId",
            "Tokens",
            "UserId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "Tokens");

        migrationBuilder.RenameColumn(
            "UpdatedAt",
            "Users",
            "Updated");

        migrationBuilder.RenameColumn(
            "CreatedAt",
            "Users",
            "Created");
    }
}