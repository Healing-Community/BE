using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_Platform_fee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlatformFees",
                columns: table => new
                {
                    PlatformFeeId = table.Column<string>(type: "text", nullable: false),
                    PlatformFeeName = table.Column<string>(type: "text", nullable: false),
                    PlatformFeeDescription = table.Column<string>(type: "text", nullable: false),
                    PlatformFeeValue = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformFees", x => x.PlatformFeeId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlatformFees");
        }
    }
}
