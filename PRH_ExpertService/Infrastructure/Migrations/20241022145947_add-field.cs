using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addfield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackIdCardUrl",
                table: "ExpertProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FrontIdCardUrl",
                table: "ExpertProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ExpertProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                table: "Certificates",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackIdCardUrl",
                table: "ExpertProfiles");

            migrationBuilder.DropColumn(
                name: "FrontIdCardUrl",
                table: "ExpertProfiles");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ExpertProfiles");

            migrationBuilder.DropColumn(
                name: "FileUrl",
                table: "Certificates");
        }
    }
}
