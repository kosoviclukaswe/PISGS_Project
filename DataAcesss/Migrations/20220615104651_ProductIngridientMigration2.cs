using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAcesss.Migrations
{
    public partial class ProductIngridientMigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingridients_Products_ProductId",
                table: "Ingridients");

            migrationBuilder.DropIndex(
                name: "IX_Ingridients_ProductId",
                table: "Ingridients");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Ingridients");

            migrationBuilder.CreateTable(
                name: "ProductIngridient",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    IngridientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductIngridient", x => new { x.ProductId, x.IngridientId });
                    table.ForeignKey(
                        name: "FK_ProductIngridient_Ingridients_IngridientId",
                        column: x => x.IngridientId,
                        principalTable: "Ingridients",
                        principalColumn: "IngridientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductIngridient_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductIngridient_IngridientId",
                table: "ProductIngridient",
                column: "IngridientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductIngridient");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Ingridients",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ingridients_ProductId",
                table: "Ingridients",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingridients_Products_ProductId",
                table: "Ingridients",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
