using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAcesss.Migrations
{
    public partial class ProductIngridientMigration3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductIngridient_Ingridients_IngridientId",
                table: "ProductIngridient");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductIngridient_Products_ProductId",
                table: "ProductIngridient");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductIngridient",
                table: "ProductIngridient");

            migrationBuilder.RenameTable(
                name: "ProductIngridient",
                newName: "ProductIngridients");

            migrationBuilder.RenameIndex(
                name: "IX_ProductIngridient_IngridientId",
                table: "ProductIngridients",
                newName: "IX_ProductIngridients_IngridientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductIngridients",
                table: "ProductIngridients",
                columns: new[] { "ProductId", "IngridientId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProductIngridients_Ingridients_IngridientId",
                table: "ProductIngridients",
                column: "IngridientId",
                principalTable: "Ingridients",
                principalColumn: "IngridientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductIngridients_Products_ProductId",
                table: "ProductIngridients",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductIngridients_Ingridients_IngridientId",
                table: "ProductIngridients");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductIngridients_Products_ProductId",
                table: "ProductIngridients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductIngridients",
                table: "ProductIngridients");

            migrationBuilder.RenameTable(
                name: "ProductIngridients",
                newName: "ProductIngridient");

            migrationBuilder.RenameIndex(
                name: "IX_ProductIngridients_IngridientId",
                table: "ProductIngridient",
                newName: "IX_ProductIngridient_IngridientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductIngridient",
                table: "ProductIngridient",
                columns: new[] { "ProductId", "IngridientId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProductIngridient_Ingridients_IngridientId",
                table: "ProductIngridient",
                column: "IngridientId",
                principalTable: "Ingridients",
                principalColumn: "IngridientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductIngridient_Products_ProductId",
                table: "ProductIngridient",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
