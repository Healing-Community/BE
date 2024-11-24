using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changedatatype3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RejectionReason",
                table: "ExpertProfiles",
                newName: "ImageUrl");

            migrationBuilder.RenameColumn(
                name: "ProfileImageUrl",
                table: "ExpertProfiles",
                newName: "Fullname");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "ExpertProfiles",
                newName: "RejectionReason");

            migrationBuilder.RenameColumn(
                name: "Fullname",
                table: "ExpertProfiles",
                newName: "ProfileImageUrl");
        }
    }
}
