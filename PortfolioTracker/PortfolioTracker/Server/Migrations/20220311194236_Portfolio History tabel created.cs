using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioTracker.Server.Migrations
{
    public partial class PortfolioHistorytabelcreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PortfolioHistory",
                columns: table => new
                {
                    PortfolioHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalPortfolioValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalInvestedPortfolioValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Percentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Profit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioHistory", x => x.PortfolioHistoryId);
                });

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 1,
                column: "UpdatedOn",
                value: new DateTime(2022, 3, 11, 20, 42, 35, 88, DateTimeKind.Local).AddTicks(1953));

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 2,
                column: "UpdatedOn",
                value: new DateTime(2022, 3, 11, 20, 42, 35, 88, DateTimeKind.Local).AddTicks(1982));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PortfolioHistory");

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
    }
}
