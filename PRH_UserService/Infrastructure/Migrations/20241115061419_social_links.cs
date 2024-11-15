using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class social_links : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SocialLink_Users_UserId",
                table: "SocialLink");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SocialLink",
                table: "SocialLink");

            migrationBuilder.RenameTable(
                name: "SocialLink",
                newName: "SocialLinks");

            migrationBuilder.RenameIndex(
                name: "IX_SocialLink_UserId",
                table: "SocialLinks",
                newName: "IX_SocialLinks_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SocialLinks",
                table: "SocialLinks",
                column: "LinkId");

            migrationBuilder.AddForeignKey(
                name: "FK_SocialLinks_Users_UserId",
                table: "SocialLinks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SocialLinks_Users_UserId",
                table: "SocialLinks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SocialLinks",
                table: "SocialLinks");

            migrationBuilder.RenameTable(
                name: "SocialLinks",
                newName: "SocialLink");

            migrationBuilder.RenameIndex(
                name: "IX_SocialLinks_UserId",
                table: "SocialLink",
                newName: "IX_SocialLink_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SocialLink",
                table: "SocialLink",
                column: "LinkId");

            migrationBuilder.AddForeignKey(
                name: "FK_SocialLink_Users_UserId",
                table: "SocialLink",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
