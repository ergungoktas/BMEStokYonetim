using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BMEStokYonetim.Migrations
{
    /// <inheritdoc />
    public partial class BMESon0012 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenancePersonnel_AspNetUsers_UserId",
                table: "MaintenancePersonnel");

            migrationBuilder.DropTable(
                name: "AkaryakitCikislar");

            migrationBuilder.DropTable(
                name: "AkaryakitGirisler");

            migrationBuilder.DropTable(
                name: "AkaryakitIstasyonlar");

            migrationBuilder.DropColumn(
                name: "RequestItemNr",
                table: "RequestItems");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Assets",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenancePersonnel_AspNetUsers_UserId",
                table: "MaintenancePersonnel",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenancePersonnel_AspNetUsers_UserId",
                table: "MaintenancePersonnel");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Assets");

            migrationBuilder.AddColumn<string>(
                name: "RequestItemNr",
                table: "RequestItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AkaryakitIstasyonlar",
                columns: table => new
                {
                    IstasyonID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tip = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AkaryakitIstasyonlar", x => x.IstasyonID);
                });

            migrationBuilder.CreateTable(
                name: "AkaryakitCikislar",
                columns: table => new
                {
                    YakitCikisID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KaynakIstasyonID = table.Column<int>(type: "int", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CalismaSaati = table.Column<int>(type: "int", nullable: true),
                    HedefID = table.Column<int>(type: "int", nullable: false),
                    HedefTip = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    KM = table.Column<int>(type: "int", nullable: true),
                    MiktarLitre = table.Column<int>(type: "int", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AkaryakitCikislar", x => x.YakitCikisID);
                    table.ForeignKey(
                        name: "FK_AkaryakitCikislar_AkaryakitIstasyonlar_KaynakIstasyonID",
                        column: x => x.KaynakIstasyonID,
                        principalTable: "AkaryakitIstasyonlar",
                        principalColumn: "IstasyonID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AkaryakitGirisler",
                columns: table => new
                {
                    YakitGirisID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IstasyonID = table.Column<int>(type: "int", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MiktarLitre = table.Column<int>(type: "int", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AkaryakitGirisler", x => x.YakitGirisID);
                    table.ForeignKey(
                        name: "FK_AkaryakitGirisler_AkaryakitIstasyonlar_IstasyonID",
                        column: x => x.IstasyonID,
                        principalTable: "AkaryakitIstasyonlar",
                        principalColumn: "IstasyonID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AkaryakitCikislar_KaynakIstasyonID",
                table: "AkaryakitCikislar",
                column: "KaynakIstasyonID");

            migrationBuilder.CreateIndex(
                name: "IX_AkaryakitGirisler_IstasyonID",
                table: "AkaryakitGirisler",
                column: "IstasyonID");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenancePersonnel_AspNetUsers_UserId",
                table: "MaintenancePersonnel",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
