using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddShareIdToReaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShareId",
                table: "Reactions",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_ShareId",
                table: "Reactions",
                column: "ShareId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reactions_Shares_ShareId",
                table: "Reactions",
                column: "ShareId",
                principalTable: "Shares",
                principalColumn: "ShareId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reactions_Shares_ShareId",
                table: "Reactions");

            migrationBuilder.DropIndex(
                name: "IX_Reactions_ShareId",
                table: "Reactions");

            migrationBuilder.DropColumn(
                name: "ShareId",
                table: "Reactions");
        }
    }
}
