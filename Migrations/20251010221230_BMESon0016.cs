using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BMEStokYonetim.Migrations
{
    /// <inheritdoc />
    public partial class BMESon0016 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Warehouses_Locations_LocationId",
                table: "Warehouses");

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouses_Locations_LocationId",
                table: "Warehouses",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Warehouses_Locations_LocationId",
                table: "Warehouses");

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouses_Locations_LocationId",
                table: "Warehouses",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id");
        }
    }
}
