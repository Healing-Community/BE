#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations;

/// <inheritdoc />
public partial class changeidname : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            "Id",
            "Users",
            "UserId");

        migrationBuilder.RenameColumn(
            "Id",
            "Roles",
            "RoleId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            "UserId",
            "Users",
            "Id");

        migrationBuilder.RenameColumn(
            "RoleId",
            "Roles",
            "Id");
    }
}