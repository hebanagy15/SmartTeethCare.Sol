using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartTeethCare.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddSlotReservationUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SlotReservations_DoctorId_Date_StartTime",
                table: "SlotReservations",
                columns: new[] { "DoctorId", "Date", "StartTime" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SlotReservations_DoctorId_Date_StartTime",
                table: "SlotReservations");
        }
    }
}
