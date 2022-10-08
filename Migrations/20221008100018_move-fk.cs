using Microsoft.EntityFrameworkCore.Migrations;

namespace farm2plate.Migrations
{
    public partial class movefk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SOrders_AspNetUsers_ApplicationUserId",
                table: "SOrders");

            migrationBuilder.DropIndex(
                name: "IX_SOrders_ApplicationUserId",
                table: "SOrders");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "SOrders");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "SOrders",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SOrders_UserID",
                table: "SOrders",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_SOrders_AspNetUsers_UserID",
                table: "SOrders",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SOrders_AspNetUsers_UserID",
                table: "SOrders");

            migrationBuilder.DropIndex(
                name: "IX_SOrders_UserID",
                table: "SOrders");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "SOrders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "SOrders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SOrders_ApplicationUserId",
                table: "SOrders",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SOrders_AspNetUsers_ApplicationUserId",
                table: "SOrders",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
