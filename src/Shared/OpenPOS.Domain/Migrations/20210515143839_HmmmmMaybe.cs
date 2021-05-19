using Microsoft.EntityFrameworkCore.Migrations;

namespace OpenPOS.Domain.Migrations
{
    public partial class HmmmmMaybe : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stores_AspNetUsers_SelectorUserId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_SelectorUserId",
                table: "Stores");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SelectedStoreId",
                table: "AspNetUsers",
                column: "SelectedStoreId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Stores_SelectedStoreId",
                table: "AspNetUsers",
                column: "SelectedStoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Stores_SelectedStoreId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SelectedStoreId",
                table: "AspNetUsers");

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
    }
}
