using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Attendance_Config : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Attendances_DoctorId",
                table: "Attendances",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_PatientId",
                table: "Attendances",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Doctors_DoctorId",
                table: "Attendances",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Patients_PatientId",
                table: "Attendances",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Doctors_DoctorId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Patients_PatientId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_DoctorId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_PatientId",
                table: "Attendances");
        }
    }
}
