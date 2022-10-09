using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace farm2plate.Migrations
{
    public partial class changeorderstatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderStatusChanges",
                columns: table => new
                {
                    OrderStatusChangeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID = table.Column<string>(nullable: true),
                    CustomerName = table.Column<string>(nullable: true),
                    ShopName = table.Column<string>(nullable: true),
                    SOrderID = table.Column<int>(nullable: false),
                    OldSOrderStatus = table.Column<int>(nullable: false),
                    NewSOrderStatus = table.Column<int>(nullable: false),
                    SOrderDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatusChanges", x => x.OrderStatusChangeID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderStatusChanges");
        }
    }
}
