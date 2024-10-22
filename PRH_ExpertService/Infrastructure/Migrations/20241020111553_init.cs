using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CertificateTypes",
                columns: table => new
                {
                    CertificateTypeId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateTypes", x => x.CertificateTypeId);
                });

            migrationBuilder.CreateTable(
                name: "ExpertProfiles",
                columns: table => new
                {
                    ExpertProfileId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ExperienceYears = table.Column<int>(type: "integer", nullable: false),
                    ExpertiseAreas = table.Column<string>(type: "text", nullable: true),
                    Bio = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpertProfiles", x => x.ExpertProfileId);
                });

            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    CertificateId = table.Column<string>(type: "text", nullable: false),
                    ExpertProfileId = table.Column<string>(type: "text", nullable: false),
                    CertificateTypeId = table.Column<string>(type: "text", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.CertificateId);
                    table.ForeignKey(
                        name: "FK_Certificates_CertificateTypes_CertificateTypeId",
                        column: x => x.CertificateTypeId,
                        principalTable: "CertificateTypes",
                        principalColumn: "CertificateTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Certificates_ExpertProfiles_ExpertProfileId",
                        column: x => x.ExpertProfileId,
                        principalTable: "ExpertProfiles",
                        principalColumn: "ExpertProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_CertificateTypeId",
                table: "Certificates",
                column: "CertificateTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_ExpertProfileId",
                table: "Certificates",
                column: "ExpertProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateTypes_Name",
                table: "CertificateTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExpertProfiles_UserId",
                table: "ExpertProfiles",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Certificates");

            migrationBuilder.DropTable(
                name: "CertificateTypes");

            migrationBuilder.DropTable(
                name: "ExpertProfiles");
        }
    }
}
