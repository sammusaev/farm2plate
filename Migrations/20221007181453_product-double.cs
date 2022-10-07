using Microsoft.EntityFrameworkCore.Migrations;

namespace farm2plate.Migrations
{
    public partial class productdouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SOrder_AspNetUsers_UserID",
                table: "SOrder");

            migrationBuilder.DropIndex(
                name: "IX_SOrder_UserID",
                table: "SOrder");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "SOrder",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "SOrder",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "ProductQuantity",
                table: "Products",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_SOrder_ApplicationUserId",
                table: "SOrder",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SOrder_AspNetUsers_ApplicationUserId",
                table: "SOrder",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SOrder_AspNetUsers_ApplicationUserId",
                table: "SOrder");

            migrationBuilder.DropIndex(
                name: "IX_SOrder_ApplicationUserId",
                table: "SOrder");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "SOrder");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "SOrder",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductQuantity",
                table: "Products",
                type: "int",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.CreateIndex(
                name: "IX_SOrder_UserID",
                table: "SOrder",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_SOrder_AspNetUsers_UserID",
                table: "SOrder",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
