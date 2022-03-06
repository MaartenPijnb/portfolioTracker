using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioTracker.Server.Migrations
{
    public partial class TranactionsfromDegirosupported : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Symbol",
                table: "Transactions");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountOfShares",
                table: "Transactions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "Transactions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerShare",
                table: "Transactions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxesCosts",
                table: "Transactions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCosts",
                table: "Transactions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TransactionCosts",
                table: "Transactions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TransactionType",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 1,
                column: "UpdatedOn",
                value: new DateTime(2022, 3, 6, 19, 23, 0, 290, DateTimeKind.Local).AddTicks(4358));

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 2,
                column: "UpdatedOn",
                value: new DateTime(2022, 3, 6, 19, 23, 0, 290, DateTimeKind.Local).AddTicks(4388));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountOfShares",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "PricePerShare",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TaxesCosts",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TotalCosts",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransactionCosts",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransactionType",
                table: "Transactions");

            migrationBuilder.AddColumn<string>(
                name: "Symbol",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 1,
                column: "UpdatedOn",
                value: new DateTime(2022, 3, 6, 18, 47, 31, 299, DateTimeKind.Local).AddTicks(7619));

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 2,
                column: "UpdatedOn",
                value: new DateTime(2022, 3, 6, 18, 47, 31, 299, DateTimeKind.Local).AddTicks(7652));
        }
    }
}
