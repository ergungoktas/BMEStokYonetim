using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BMEStokYonetim.Migrations
{
    /// <inheritdoc />
    public partial class BMESon010 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoExpiryDays",
                table: "StockReservations");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "StockReservations");

            migrationBuilder.DropColumn(
                name: "ReservationDate",
                table: "StockReservations");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "StockReservations",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "IsAutoCreated",
                table: "StockReservations",
                newName: "IsActive");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "StockReservations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "StockReservations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReleasedAt",
                table: "StockReservations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReservedQuantity",
                table: "StockReservations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StockReservations_CreatedByUserId",
                table: "StockReservations",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockReservations_AspNetUsers_CreatedByUserId",
                table: "StockReservations",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockReservations_AspNetUsers_CreatedByUserId",
                table: "StockReservations");

            migrationBuilder.DropIndex(
                name: "IX_StockReservations_CreatedByUserId",
                table: "StockReservations");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "StockReservations");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "StockReservations");

            migrationBuilder.DropColumn(
                name: "ReleasedAt",
                table: "StockReservations");

            migrationBuilder.DropColumn(
                name: "ReservedQuantity",
                table: "StockReservations");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "StockReservations",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "StockReservations",
                newName: "IsAutoCreated");

            migrationBuilder.AddColumn<int>(
                name: "AutoExpiryDays",
                table: "StockReservations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ExpiryDate",
                table: "StockReservations",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "ReservationDate",
                table: "StockReservations",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }
    }
}
