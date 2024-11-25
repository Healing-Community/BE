using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_user_preferences_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPreference_Categories_CategoryId",
                table: "UserPreference");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPreference",
                table: "UserPreference");

            migrationBuilder.RenameTable(
                name: "UserPreference",
                newName: "UserPreferences");

            migrationBuilder.RenameIndex(
                name: "IX_UserPreference_CategoryId",
                table: "UserPreferences",
                newName: "IX_UserPreferences_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPreferences",
                table: "UserPreferences",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreferences_Categories_CategoryId",
                table: "UserPreferences",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPreferences_Categories_CategoryId",
                table: "UserPreferences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPreferences",
                table: "UserPreferences");

            migrationBuilder.RenameTable(
                name: "UserPreferences",
                newName: "UserPreference");

            migrationBuilder.RenameIndex(
                name: "IX_UserPreferences_CategoryId",
                table: "UserPreference",
                newName: "IX_UserPreference_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPreference",
                table: "UserPreference",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreference_Categories_CategoryId",
                table: "UserPreference",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
