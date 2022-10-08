using Microsoft.EntityFrameworkCore.Migrations;

namespace farm2plate.Migrations
{
    public partial class dbsetsorders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SOrder_AspNetUsers_ApplicationUserId",
                table: "SOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_SOrder_Products_ProductID",
                table: "SOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_SOrder_Shops_ShopID",
                table: "SOrder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SOrder",
                table: "SOrder");

            migrationBuilder.RenameTable(
                name: "SOrder",
                newName: "SOrders");

            migrationBuilder.RenameIndex(
                name: "IX_SOrder_ShopID",
                table: "SOrders",
                newName: "IX_SOrders_ShopID");

            migrationBuilder.RenameIndex(
                name: "IX_SOrder_ProductID",
                table: "SOrders",
                newName: "IX_SOrders_ProductID");

            migrationBuilder.RenameIndex(
                name: "IX_SOrder_ApplicationUserId",
                table: "SOrders",
                newName: "IX_SOrders_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SOrders",
                table: "SOrders",
                column: "SOrderID");

            migrationBuilder.AddForeignKey(
                name: "FK_SOrders_AspNetUsers_ApplicationUserId",
                table: "SOrders",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SOrders_Products_ProductID",
                table: "SOrders",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SOrders_Shops_ShopID",
                table: "SOrders",
                column: "ShopID",
                principalTable: "Shops",
                principalColumn: "ShopID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SOrders_AspNetUsers_ApplicationUserId",
                table: "SOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_SOrders_Products_ProductID",
                table: "SOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_SOrders_Shops_ShopID",
                table: "SOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SOrders",
                table: "SOrders");

            migrationBuilder.RenameTable(
                name: "SOrders",
                newName: "SOrder");

            migrationBuilder.RenameIndex(
                name: "IX_SOrders_ShopID",
                table: "SOrder",
                newName: "IX_SOrder_ShopID");

            migrationBuilder.RenameIndex(
                name: "IX_SOrders_ProductID",
                table: "SOrder",
                newName: "IX_SOrder_ProductID");

            migrationBuilder.RenameIndex(
                name: "IX_SOrders_ApplicationUserId",
                table: "SOrder",
                newName: "IX_SOrder_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SOrder",
                table: "SOrder",
                column: "SOrderID");

            migrationBuilder.AddForeignKey(
                name: "FK_SOrder_AspNetUsers_ApplicationUserId",
                table: "SOrder",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SOrder_Products_ProductID",
                table: "SOrder",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SOrder_Shops_ShopID",
                table: "SOrder",
                column: "ShopID",
                principalTable: "Shops",
                principalColumn: "ShopID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
