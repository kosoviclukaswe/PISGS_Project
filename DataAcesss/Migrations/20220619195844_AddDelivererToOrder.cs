﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAcesss.Migrations
{
    public partial class AddDelivererToOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DelivererId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DelivererId",
                table: "Orders",
                column: "DelivererId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_DelivererId",
                table: "Orders",
                column: "DelivererId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_DelivererId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_DelivererId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DelivererId",
                table: "Orders");
        }
    }
}
