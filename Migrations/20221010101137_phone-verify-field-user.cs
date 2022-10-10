using Microsoft.EntityFrameworkCore.Migrations;

namespace farm2plate.Migrations
{
    public partial class phoneverifyfielduser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PhoneIsVerified",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneIsVerified",
                table: "AspNetUsers");
        }
    }
}
