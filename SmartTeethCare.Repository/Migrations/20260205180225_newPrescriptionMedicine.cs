using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartTeethCare.Repository.Migrations
{
    /// <inheritdoc />
    public partial class newPrescriptionMedicine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppointmentId",
                table: "Prescriptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Dosage",
                table: "PrescriptionMedicines",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DurationInDays",
                table: "PrescriptionMedicines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Frequency",
                table: "PrescriptionMedicines",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "PrescriptionMedicines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "PrescriptionMedicines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Appointments",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_AppointmentId",
                table: "Prescriptions",
                column: "AppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prescriptions_Appointments_AppointmentId",
                table: "Prescriptions",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prescriptions_Appointments_AppointmentId",
                table: "Prescriptions");

            migrationBuilder.DropIndex(
                name: "IX_Prescriptions_AppointmentId",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "AppointmentId",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "Dosage",
                table: "PrescriptionMedicines");

            migrationBuilder.DropColumn(
                name: "DurationInDays",
                table: "PrescriptionMedicines");

            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "PrescriptionMedicines");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PrescriptionMedicines");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "PrescriptionMedicines");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
