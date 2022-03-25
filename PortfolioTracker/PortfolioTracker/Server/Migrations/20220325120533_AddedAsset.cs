using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioTracker.Server.Migrations
{
    public partial class AddedAsset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "Assets",
                columns: new[] { "AssetId", "APIId", "ISN", "Name", "SymbolForApi", "UpdatedOn", "Value" },
                values: new object[] { 3, 1, "BE0172903495", "Argenta Pensioenspaarfonds", "0P00000NFB.F", new DateTime(2022, 3, 25, 13, 5, 32, 274, DateTimeKind.Local).AddTicks(1050), 0m });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 1,
                column: "UpdatedOn",
                value: new DateTime(2022, 3, 16, 20, 32, 52, 193, DateTimeKind.Local).AddTicks(7973));

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 2,
                column: "UpdatedOn",
                value: new DateTime(2022, 3, 16, 20, 32, 52, 193, DateTimeKind.Local).AddTicks(8019));
        }
    }
}
