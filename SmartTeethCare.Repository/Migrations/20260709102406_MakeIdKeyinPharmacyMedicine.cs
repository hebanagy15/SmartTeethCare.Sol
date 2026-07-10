using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartTeethCare.Repository.Migrations
{
    /// <inheritdoc />
    public partial class MakeIdKeyinPharmacyMedicine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PharmacyMedicines",
                table: "PharmacyMedicines");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PharmacyMedicines");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "PharmacyMedicines",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PharmacyMedicines",
                table: "PharmacyMedicines",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PharmacyMedicines_PharmacyID_MedicineID",
                table: "PharmacyMedicines",
                columns: new[] { "PharmacyID", "MedicineID" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PharmacyMedicines",
                table: "PharmacyMedicines");

            migrationBuilder.DropIndex(
                name: "IX_PharmacyMedicines_PharmacyID_MedicineID",
                table: "PharmacyMedicines");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "PharmacyMedicines",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PharmacyMedicines",
                table: "PharmacyMedicines",
                columns: new[] { "PharmacyID", "MedicineID" });
        }
    }
}
