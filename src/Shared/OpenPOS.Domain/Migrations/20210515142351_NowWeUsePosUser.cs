using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OpenPOS.Domain.Migrations
{
    public partial class NowWeUsePosUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "SelectorUserId",
                table: "Stores",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SelectedStoreId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stores_SelectorUserId",
                table: "Stores",
                column: "SelectorUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_AspNetUsers_SelectorUserId",
                table: "Stores",
                column: "SelectorUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stores_AspNetUsers_SelectorUserId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_SelectorUserId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "SelectorUserId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "SelectedStoreId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
