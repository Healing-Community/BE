#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations;

/// <inheritdoc />
public partial class addpasswordhash : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            "Description",
            "Roles");

        migrationBuilder.RenameColumn(
            "status",
            "Users",
            "Status");

        migrationBuilder.AddColumn<string>(
            "PasswordHash",
            "Users",
            "text",
            nullable: false,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            "PasswordHash",
            "Users");

        migrationBuilder.RenameColumn(
            "Status",
            "Users",
            "status");

        migrationBuilder.AddColumn<string>(
            "Description",
            "Roles",
            "text",
            nullable: false,
            defaultValue: "");
    }
}