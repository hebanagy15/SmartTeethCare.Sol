using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartTeethCare.Repository.Migrations
{
    /// <inheritdoc />
    public partial class addDiseaseToSpeciality : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Disease",
                table: "Specialities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Disease",
                table: "Specialities");
        }
    }
}
