using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OpenPOS.Domain.Migrations
{
    public partial class AddFirms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Firm_FirmId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Firm_FirmId",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Firm",
                table: "Firm");

            migrationBuilder.RenameTable(
                name: "Firm",
                newName: "Firms");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Barcode",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "Firms",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Firms",
                table: "Firms",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Firms_StoreId",
                table: "Firms",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Firms_Stores_StoreId",
                table: "Firms",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Firms_FirmId",
                table: "Products",
                column: "FirmId",
                principalTable: "Firms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Firms_FirmId",
                table: "Transactions",
                column: "FirmId",
                principalTable: "Firms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Firms_Stores_StoreId",
                table: "Firms");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Firms_FirmId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Firms_FirmId",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Firms",
                table: "Firms");

            migrationBuilder.DropIndex(
                name: "IX_Firms_StoreId",
                table: "Firms");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Firms");

            migrationBuilder.RenameTable(
                name: "Firms",
                newName: "Firm");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Barcode",
                table: "Products",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Firm",
                table: "Firm",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Firm_FirmId",
                table: "Products",
                column: "FirmId",
                principalTable: "Firm",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Firm_FirmId",
                table: "Transactions",
                column: "FirmId",
                principalTable: "Firm",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
