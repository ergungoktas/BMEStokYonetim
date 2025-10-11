using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BMEStokYonetim.Migrations
{
    /// <inheritdoc />
    public partial class BMEson0018 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetDailyChecks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CheckDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Km = table.Column<int>(type: "int", nullable: true),
                    HourMeter = table.Column<int>(type: "int", nullable: true),
                    GeneralNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EngineOilOk = table.Column<bool>(type: "bit", nullable: true),
                    EngineOilNotes = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    HydraulicOilOk = table.Column<bool>(type: "bit", nullable: true),
                    HydraulicOilNotes = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CoolantOk = table.Column<bool>(type: "bit", nullable: true),
                    CoolantNotes = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TireConditionOk = table.Column<bool>(type: "bit", nullable: true),
                    TireNotes = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    LightsOk = table.Column<bool>(type: "bit", nullable: true),
                    LightsNotes = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    HornOk = table.Column<bool>(type: "bit", nullable: true),
                    HornNotes = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    SafetyEquipmentsOk = table.Column<bool>(type: "bit", nullable: true),
                    SafetyEquipmentsNotes = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    HasFault = table.Column<bool>(type: "bit", nullable: true),
                    FaultDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PhotoPath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetDailyChecks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetDailyChecks_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetDailyChecks_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetResponsibilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetResponsibilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetResponsibilities_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetResponsibilities_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetDailyChecks_AssetId_UserId_CheckDate",
                table: "AssetDailyChecks",
                columns: new[] { "AssetId", "UserId", "CheckDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetDailyChecks_UserId",
                table: "AssetDailyChecks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetResponsibilities_AssetId",
                table: "AssetResponsibilities",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetResponsibilities_UserId",
                table: "AssetResponsibilities",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetDailyChecks");

            migrationBuilder.DropTable(
                name: "AssetResponsibilities");
        }
    }
}
