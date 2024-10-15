using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fix_post_name : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Costs_Categories_CategoryId",
                table: "Costs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Costs",
                table: "Costs");

            migrationBuilder.RenameTable(
                name: "Costs",
                newName: "Posts");

            migrationBuilder.RenameIndex(
                name: "IX_Costs_Title",
                table: "Posts",
                newName: "IX_Posts_Title");

            migrationBuilder.RenameIndex(
                name: "IX_Costs_CategoryId",
                table: "Posts",
                newName: "IX_Posts_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Posts",
                table: "Posts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Categories_CategoryId",
                table: "Posts",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Categories_CategoryId",
                table: "Posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Posts",
                table: "Posts");

            migrationBuilder.RenameTable(
                name: "Posts",
                newName: "Costs");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_Title",
                table: "Costs",
                newName: "IX_Costs_Title");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_CategoryId",
                table: "Costs",
                newName: "IX_Costs_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Costs",
                table: "Costs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Costs_Categories_CategoryId",
                table: "Costs",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
