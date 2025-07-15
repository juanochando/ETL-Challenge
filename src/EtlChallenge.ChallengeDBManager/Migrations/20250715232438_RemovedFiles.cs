using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EtlChallenge.ChallengeDBManager.Migrations
{
    /// <inheritdoc />
    public partial class RemovedFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Policies_PolicyFiles_PolicyFileStorageReference",
                table: "Policies");

            migrationBuilder.DropForeignKey(
                name: "FK_Risks_RiskFiles_RiskFileStorageReference",
                table: "Risks");

            migrationBuilder.DropForeignKey(
                name: "FK_StagedPolicies_PolicyFiles_PolicyFileStorageReference",
                table: "StagedPolicies");

            migrationBuilder.DropForeignKey(
                name: "FK_StagedRisks_RiskFiles_RiskFileStorageReference",
                table: "StagedRisks");

            migrationBuilder.DropTable(
                name: "PolicyFiles");

            migrationBuilder.DropTable(
                name: "RiskFiles");

            migrationBuilder.DropIndex(
                name: "IX_StagedRisks_RiskFileStorageReference",
                table: "StagedRisks");

            migrationBuilder.DropIndex(
                name: "IX_StagedPolicies_PolicyFileStorageReference",
                table: "StagedPolicies");

            migrationBuilder.DropIndex(
                name: "IX_Risks_RiskFileStorageReference",
                table: "Risks");

            migrationBuilder.DropIndex(
                name: "IX_Policies_PolicyFileStorageReference",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "RiskFileStorageReference",
                table: "StagedRisks");

            migrationBuilder.DropColumn(
                name: "PolicyFileStorageReference",
                table: "StagedPolicies");

            migrationBuilder.DropColumn(
                name: "RiskFileStorageReference",
                table: "Risks");

            migrationBuilder.DropColumn(
                name: "PolicyFileStorageReference",
                table: "Policies");

            migrationBuilder.AddColumn<string>(
                name: "FileStorageReference",
                table: "StagedRisks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileStorageReference",
                table: "StagedPolicies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileStorageReference",
                table: "Risks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileStorageReference",
                table: "Policies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileStorageReference",
                table: "StagedRisks");

            migrationBuilder.DropColumn(
                name: "FileStorageReference",
                table: "StagedPolicies");

            migrationBuilder.DropColumn(
                name: "FileStorageReference",
                table: "Risks");

            migrationBuilder.DropColumn(
                name: "FileStorageReference",
                table: "Policies");

            migrationBuilder.AddColumn<string>(
                name: "RiskFileStorageReference",
                table: "StagedRisks",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PolicyFileStorageReference",
                table: "StagedPolicies",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RiskFileStorageReference",
                table: "Risks",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PolicyFileStorageReference",
                table: "Policies",
                type: "nvarchar(450)",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_StagedRisks_RiskFileStorageReference",
                table: "StagedRisks",
                column: "RiskFileStorageReference");

            migrationBuilder.CreateIndex(
                name: "IX_StagedPolicies_PolicyFileStorageReference",
                table: "StagedPolicies",
                column: "PolicyFileStorageReference");

            migrationBuilder.CreateIndex(
                name: "IX_Risks_RiskFileStorageReference",
                table: "Risks",
                column: "RiskFileStorageReference");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_PolicyFileStorageReference",
                table: "Policies",
                column: "PolicyFileStorageReference");

            migrationBuilder.AddForeignKey(
                name: "FK_Policies_PolicyFiles_PolicyFileStorageReference",
                table: "Policies",
                column: "PolicyFileStorageReference",
                principalTable: "PolicyFiles",
                principalColumn: "StorageReference");

            migrationBuilder.AddForeignKey(
                name: "FK_Risks_RiskFiles_RiskFileStorageReference",
                table: "Risks",
                column: "RiskFileStorageReference",
                principalTable: "RiskFiles",
                principalColumn: "StorageReference");

            migrationBuilder.AddForeignKey(
                name: "FK_StagedPolicies_PolicyFiles_PolicyFileStorageReference",
                table: "StagedPolicies",
                column: "PolicyFileStorageReference",
                principalTable: "PolicyFiles",
                principalColumn: "StorageReference");

            migrationBuilder.AddForeignKey(
                name: "FK_StagedRisks_RiskFiles_RiskFileStorageReference",
                table: "StagedRisks",
                column: "RiskFileStorageReference",
                principalTable: "RiskFiles",
                principalColumn: "StorageReference");
        }
    }
}
