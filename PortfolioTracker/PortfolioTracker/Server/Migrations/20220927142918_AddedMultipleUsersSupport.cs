using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioTracker.Server.Migrations
{
    public partial class AddedMultipleUsersSupport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserID",
                table: "Transactions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UserID",
                table: "PortfolioHistory",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UserID",
                table: "Portfolio",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UserID",
                table: "AccountBalance",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "PortfolioHistory");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Portfolio");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "AccountBalance");

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 1,
                column: "UpdatedOn",
                value: new DateTime(2022, 6, 26, 10, 34, 19, 958, DateTimeKind.Local).AddTicks(1482));

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 2,
                column: "UpdatedOn",
                value: new DateTime(2022, 6, 26, 10, 34, 19, 958, DateTimeKind.Local).AddTicks(1517));

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 3,
                column: "UpdatedOn",
                value: new DateTime(2022, 6, 26, 10, 34, 19, 958, DateTimeKind.Local).AddTicks(1586));

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 4,
                column: "UpdatedOn",
                value: new DateTime(2022, 6, 26, 10, 34, 19, 958, DateTimeKind.Local).AddTicks(1589));
        }
    }
}
