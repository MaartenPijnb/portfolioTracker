using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioTracker.Server.Migrations
{
    public partial class PortfolioTableUpdteAttribute : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalInvestedValue",
                table: "Portfolio",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 1,
                column: "UpdatedOn",
                value: new DateTime(2022, 3, 12, 18, 43, 10, 334, DateTimeKind.Local).AddTicks(8886));

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 2,
                column: "UpdatedOn",
                value: new DateTime(2022, 3, 12, 18, 43, 10, 334, DateTimeKind.Local).AddTicks(8914));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalInvestedValue",
                table: "Portfolio");

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 1,
                column: "UpdatedOn",
                value: new DateTime(2022, 3, 12, 18, 30, 3, 443, DateTimeKind.Local).AddTicks(4139));

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 2,
                column: "UpdatedOn",
                value: new DateTime(2022, 3, 12, 18, 30, 3, 443, DateTimeKind.Local).AddTicks(4172));
        }
    }
}
