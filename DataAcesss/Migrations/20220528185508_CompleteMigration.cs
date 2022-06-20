using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAcesss.Migrations
{
    public partial class CompleteMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SignUpRequest");

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "SignUpRequest",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SignUpRequest_AppUserId",
                table: "SignUpRequest",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SignUpRequest_AspNetUsers_AppUserId",
                table: "SignUpRequest",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SignUpRequest_AspNetUsers_AppUserId",
                table: "SignUpRequest");

            migrationBuilder.DropIndex(
                name: "IX_SignUpRequest_AppUserId",
                table: "SignUpRequest");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "SignUpRequest");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "SignUpRequest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
