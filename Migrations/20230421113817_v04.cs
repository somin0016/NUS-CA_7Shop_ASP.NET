using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP.NET_CA_SEVEN_SHOP.Migrations
{
    /// <inheritdoc />
    public partial class v04 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivationCodes_OrderDetails_OrderDetailOrderId_OrderDetailProductId",
                table: "ActivationCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivationCodes_Orders_OrderId",
                table: "ActivationCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivationCodes_Products_ProductId",
                table: "ActivationCodes");

            migrationBuilder.DropIndex(
                name: "IX_ActivationCodes_OrderId",
                table: "ActivationCodes");

            migrationBuilder.DropIndex(
                name: "IX_ActivationCodes_ProductId",
                table: "ActivationCodes");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "ActivationCodes");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ActivationCodes");

            migrationBuilder.AlterColumn<int>(
                name: "OrderDetailProductId",
                table: "ActivationCodes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OrderDetailOrderId",
                table: "ActivationCodes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivationCodes_OrderDetails_OrderDetailOrderId_OrderDetailProductId",
                table: "ActivationCodes",
                columns: new[] { "OrderDetailOrderId", "OrderDetailProductId" },
                principalTable: "OrderDetails",
                principalColumns: new[] { "OrderId", "ProductId" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivationCodes_OrderDetails_OrderDetailOrderId_OrderDetailProductId",
                table: "ActivationCodes");

            migrationBuilder.AlterColumn<int>(
                name: "OrderDetailProductId",
                table: "ActivationCodes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "OrderDetailOrderId",
                table: "ActivationCodes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "ActivationCodes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "ActivationCodes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ActivationCodes_OrderId",
                table: "ActivationCodes",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivationCodes_ProductId",
                table: "ActivationCodes",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivationCodes_OrderDetails_OrderDetailOrderId_OrderDetailProductId",
                table: "ActivationCodes",
                columns: new[] { "OrderDetailOrderId", "OrderDetailProductId" },
                principalTable: "OrderDetails",
                principalColumns: new[] { "OrderId", "ProductId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ActivationCodes_Orders_OrderId",
                table: "ActivationCodes",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivationCodes_Products_ProductId",
                table: "ActivationCodes",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
