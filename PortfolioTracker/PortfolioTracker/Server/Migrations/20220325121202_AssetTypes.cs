using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioTracker.Server.Migrations
{
    public partial class AssetTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssetType",
                table: "Assets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 1,
                columns: new[] { "AssetType", "UpdatedOn" },
                values: new object[] { 1, new DateTime(2022, 3, 25, 13, 12, 0, 813, DateTimeKind.Local).AddTicks(8203) });

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 2,
                columns: new[] { "AssetType", "UpdatedOn" },
                values: new object[] { 1, new DateTime(2022, 3, 25, 13, 12, 0, 813, DateTimeKind.Local).AddTicks(8252) });

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 3,
                columns: new[] { "AssetType", "UpdatedOn" },
                values: new object[] { 4, new DateTime(2022, 3, 25, 13, 12, 0, 813, DateTimeKind.Local).AddTicks(8255) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssetType",
                table: "Assets");

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 1,
                column: "UpdatedOn",
                value: new DateTime(2022, 3, 25, 13, 5, 32, 274, DateTimeKind.Local).AddTicks(1009));

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 2,
                column: "UpdatedOn",
                value: new DateTime(2022, 3, 25, 13, 5, 32, 274, DateTimeKind.Local).AddTicks(1047));

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 3,
                column: "UpdatedOn",
                value: new DateTime(2022, 3, 25, 13, 5, 32, 274, DateTimeKind.Local).AddTicks(1050));
        }
    }
}
