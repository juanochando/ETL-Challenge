using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtlChallenge.ChallengeDBManager.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PolicyFiles",
                columns: table => new
                {
                    StorageReference = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyFiles", x => x.StorageReference);
                });

            migrationBuilder.CreateTable(
                name: "RiskFiles",
                columns: table => new
                {
                    StorageReference = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskFiles", x => x.StorageReference);
                });

            migrationBuilder.CreateTable(
                name: "Policies",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PolicyFileStorageReference = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Policies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Policies_PolicyFiles_PolicyFileStorageReference",
                        column: x => x.PolicyFileStorageReference,
                        principalTable: "PolicyFiles",
                        principalColumn: "StorageReference");
                });

            migrationBuilder.CreateTable(
                name: "StagedPolicies",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PolicyFileStorageReference = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StagedPolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StagedPolicies_PolicyFiles_PolicyFileStorageReference",
                        column: x => x.PolicyFileStorageReference,
                        principalTable: "PolicyFiles",
                        principalColumn: "StorageReference");
                });

            migrationBuilder.CreateTable(
                name: "StagedRisks",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Peril = table.Column<int>(type: "int", nullable: false),
                    PolicyId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    RiskFileStorageReference = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StagedRisks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StagedRisks_RiskFiles_RiskFileStorageReference",
                        column: x => x.RiskFileStorageReference,
                        principalTable: "RiskFiles",
                        principalColumn: "StorageReference");
                });

            migrationBuilder.CreateTable(
                name: "Risks",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Peril = table.Column<int>(type: "int", nullable: false),
                    PolicyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    RiskFileStorageReference = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Risks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Risks_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Risks_RiskFiles_RiskFileStorageReference",
                        column: x => x.RiskFileStorageReference,
                        principalTable: "RiskFiles",
                        principalColumn: "StorageReference");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Policies_PolicyFileStorageReference",
                table: "Policies",
                column: "PolicyFileStorageReference");

            migrationBuilder.CreateIndex(
                name: "IX_Risks_PolicyId",
                table: "Risks",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_Risks_RiskFileStorageReference",
                table: "Risks",
                column: "RiskFileStorageReference");

            migrationBuilder.CreateIndex(
                name: "IX_StagedPolicies_PolicyFileStorageReference",
                table: "StagedPolicies",
                column: "PolicyFileStorageReference");

            migrationBuilder.CreateIndex(
                name: "IX_StagedRisks_RiskFileStorageReference",
                table: "StagedRisks",
                column: "RiskFileStorageReference");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Risks");

            migrationBuilder.DropTable(
                name: "StagedPolicies");

            migrationBuilder.DropTable(
                name: "StagedRisks");

            migrationBuilder.DropTable(
                name: "Policies");

            migrationBuilder.DropTable(
                name: "RiskFiles");

            migrationBuilder.DropTable(
                name: "PolicyFiles");
        }
    }
}
