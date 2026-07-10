using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartTeethCare.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintToAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Appointments_DoctorID",
                table: "Appointments");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorID_Date_StartTime",
                table: "Appointments",
                columns: new[] { "DoctorID", "Date", "StartTime" },
                unique: true,
                filter: "[Status] <> 2 AND [Status] <> 4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Appointments_DoctorID_Date_StartTime",
                table: "Appointments");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorID",
                table: "Appointments",
                column: "DoctorID");
        }
    }
}
