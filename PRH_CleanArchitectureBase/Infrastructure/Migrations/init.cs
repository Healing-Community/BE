#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations;

/// <inheritdoc />
public partial class init : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "ReportTypes",
            table => new
            {
                ReportTypeId = table.Column<string>("text", nullable: false),
                Name = table.Column<string>("character varying(100)", maxLength: 100, nullable: false),
                Description = table.Column<string>("character varying(200)", maxLength: 200, nullable: false)
            },
            constraints: table => { table.PrimaryKey("PK_ReportTypes", x => x.ReportTypeId); });

        migrationBuilder.CreateTable(
            "Reports",
            table => new
            {
                ReportId = table.Column<Guid>("uuid", nullable: false),
                UserId = table.Column<Guid>("uuid", nullable: false),
                TargetUserId = table.Column<Guid>("uuid", nullable: false),
                ExpertId = table.Column<Guid>("uuid", nullable: false),
                PostId = table.Column<Guid>("uuid", nullable: false),
                CommentId = table.Column<Guid>("uuid", nullable: false),
                ReportTypeId = table.Column<string>("text", nullable: false),
                Description = table.Column<string>("text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Reports", x => x.ReportId);
                table.ForeignKey(
                    "FK_Reports_ReportTypes_ReportTypeId",
                    x => x.ReportTypeId,
                    "ReportTypes",
                    "ReportTypeId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            "IX_Reports_ReportTypeId",
            "Reports",
            "ReportTypeId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "Reports");

        migrationBuilder.DropTable(
            "ReportTypes");
    }
}