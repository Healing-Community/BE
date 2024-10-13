#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Infrastructure.Migrations;

/// <inheritdoc />
public partial class init : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "Roles",
            table => new
            {
                Id = table.Column<int>("integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>("text", nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_Roles", x => x.Id); });

        migrationBuilder.CreateTable(
            "Users",
            table => new
            {
                Id = table.Column<Guid>("uuid", nullable: false),
                FullName = table.Column<string>("text", nullable: false),
                UserName = table.Column<string>("text", nullable: false),
                Email = table.Column<string>("text", nullable: false),
                status = table.Column<int>("integer", nullable: false),
                Created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                Updated = table.Column<DateTime>("timestamp with time zone", nullable: false),
                RoleId = table.Column<int>("integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
                table.ForeignKey(
                    "FK_Users_Roles_RoleId",
                    x => x.RoleId,
                    "Roles",
                    "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            "IX_Roles_Name",
            "Roles",
            "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            "IX_Users_RoleId",
            "Users",
            "RoleId");

        migrationBuilder.CreateIndex(
            "IX_Users_UserName",
            "Users",
            "UserName",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "Users");

        migrationBuilder.DropTable(
            "Roles");
    }
}