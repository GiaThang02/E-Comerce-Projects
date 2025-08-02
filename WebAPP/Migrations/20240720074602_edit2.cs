using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPP.Migrations
{
    public partial class edit2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductModelId",
                table: "OrderDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductModelId",
                table: "OrderDetails",
                column: "ProductModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Products_ProductModelId",
                table: "OrderDetails",
                column: "ProductModelId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Products_ProductModelId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ProductModelId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "ProductModelId",
                table: "OrderDetails");
        }
    }
}
