using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BMEStokYonetim.Migrations
{
    /// <inheritdoc />
    public partial class AddMaintenanceFaultCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FaultCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaultCodes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FaultCodes_Code",
                table: "FaultCodes",
                column: "Code",
                unique: true);

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenancePersonnel_AspNetUsers_UserId",
                table: "MaintenancePersonnel");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Maintenances");

            migrationBuilder.AddColumn<int>(
                name: "FaultCodeId",
                table: "Maintenances",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkNotes",
                table: "Maintenances",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "MaintenancePersonnel",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "MaintenancePersonnel",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_Maintenances_FaultCodeId",
                table: "Maintenances",
                column: "FaultCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenancePersonnel_AspNetUsers_UserId",
                table: "MaintenancePersonnel",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_FaultCodes_FaultCodeId",
                table: "Maintenances",
                column: "FaultCodeId",
                principalTable: "FaultCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenancePersonnel_AspNetUsers_UserId",
                table: "MaintenancePersonnel");

            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_FaultCodes_FaultCodeId",
                table: "Maintenances");

            migrationBuilder.DropIndex(
                name: "IX_Maintenances_FaultCodeId",
                table: "Maintenances");

            migrationBuilder.DropIndex(
                name: "IX_FaultCodes_Code",
                table: "FaultCodes");

            migrationBuilder.DropColumn(
                name: "FaultCodeId",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "WorkNotes",
                table: "Maintenances");

            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "Maintenances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "MaintenancePersonnel",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "MaintenancePersonnel",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.DropTable(
                name: "FaultCodes");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenancePersonnel_AspNetUsers_UserId",
                table: "MaintenancePersonnel",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
