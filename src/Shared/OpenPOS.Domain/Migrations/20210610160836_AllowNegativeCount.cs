using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OpenPOS.Domain.Migrations
{
    public partial class AllowNegativeCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_Transactions_TransactionId",
                table: "ProductVariants");

            migrationBuilder.AlterColumn<Guid>(
                name: "TransactionId",
                table: "ProductVariants",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_Transactions_TransactionId",
                table: "ProductVariants",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_Transactions_TransactionId",
                table: "ProductVariants");

            migrationBuilder.AlterColumn<Guid>(
                name: "TransactionId",
                table: "ProductVariants",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_Transactions_TransactionId",
                table: "ProductVariants",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
