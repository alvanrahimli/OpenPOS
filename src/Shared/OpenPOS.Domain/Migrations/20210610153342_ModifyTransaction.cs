using Microsoft.EntityFrameworkCore.Migrations;

namespace OpenPOS.Domain.Migrations
{
    public partial class ModifyTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PayedAmount",
                table: "Transactions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReturnAmount",
                table: "Transactions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayedAmount",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ReturnAmount",
                table: "Transactions");
        }
    }
}
