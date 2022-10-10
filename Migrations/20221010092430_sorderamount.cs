using Microsoft.EntityFrameworkCore.Migrations;

namespace farm2plate.Migrations
{
    public partial class sorderamount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "SorderAmount",
                table: "SOrders",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SorderAmount",
                table: "SOrders");
        }
    }
}
