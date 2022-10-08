using Microsoft.EntityFrameworkCore.Migrations;

namespace farm2plate.Migrations
{
    public partial class int2doublesorder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "ProductQuantity",
                table: "SOrders",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ProductQuantity",
                table: "SOrders",
                type: "int",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
