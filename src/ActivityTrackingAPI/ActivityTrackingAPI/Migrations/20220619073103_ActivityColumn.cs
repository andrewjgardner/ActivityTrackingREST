using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ActivityTrackingAPI.Migrations
{
    public partial class ActivityColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateTimeEnded",
                table: "Activities",
                newName: "DateTimeFinished");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateTimeFinished",
                table: "Activities",
                newName: "DateTimeEnded");
        }
    }
}
