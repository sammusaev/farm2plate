using Microsoft.EntityFrameworkCore.Migrations;

namespace farm2plate.Migrations
{
    public partial class sorderunitprice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "SorderUnitPrice",
                table: "SOrders",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SorderUnitPrice",
                table: "SOrders");
        }
    }
}
