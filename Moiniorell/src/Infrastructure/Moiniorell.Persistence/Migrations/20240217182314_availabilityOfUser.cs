using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moiniorell.Persistence.Migrations
{
    public partial class availabilityOfUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsOnline",
                table: "AspNetUsers",
                newName: "Availability");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Availability",
                table: "AspNetUsers",
                newName: "IsOnline");
        }
    }
}
