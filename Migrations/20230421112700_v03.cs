using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP.NET_CA_SEVEN_SHOP.Migrations
{
    /// <inheritdoc />
    public partial class v03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderDetailOrderId",
                table: "ActivationCodes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderDetailProductId",
                table: "ActivationCodes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivationCodes_OrderDetailOrderId_OrderDetailProductId",
                table: "ActivationCodes",
                columns: new[] { "OrderDetailOrderId", "OrderDetailProductId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ActivationCodes_OrderDetails_OrderDetailOrderId_OrderDetailProductId",
                table: "ActivationCodes",
                columns: new[] { "OrderDetailOrderId", "OrderDetailProductId" },
                principalTable: "OrderDetails",
                principalColumns: new[] { "OrderId", "ProductId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivationCodes_OrderDetails_OrderDetailOrderId_OrderDetailProductId",
                table: "ActivationCodes");

            migrationBuilder.DropIndex(
                name: "IX_ActivationCodes_OrderDetailOrderId_OrderDetailProductId",
                table: "ActivationCodes");

            migrationBuilder.DropColumn(
                name: "OrderDetailOrderId",
                table: "ActivationCodes");

            migrationBuilder.DropColumn(
                name: "OrderDetailProductId",
                table: "ActivationCodes");
        }
    }
}
