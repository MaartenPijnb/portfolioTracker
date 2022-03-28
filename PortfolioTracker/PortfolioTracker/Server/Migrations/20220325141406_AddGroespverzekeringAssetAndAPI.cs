using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioTracker.Server.Migrations
{
    public partial class AddGroespverzekeringAssetAndAPI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "APIs",
                keyColumn: "APIId",
                keyValue: 1,
                column: "APIName",
                value: 0);                   

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 1,
                column: "UpdatedOn",
                value: new DateTime(2022, 3, 25, 15, 14, 5, 242, DateTimeKind.Local).AddTicks(8392));

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 2,
                column: "UpdatedOn",
                value: new DateTime(2022, 3, 25, 15, 14, 5, 242, DateTimeKind.Local).AddTicks(8437));

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 3,
                column: "UpdatedOn",
                value: new DateTime(2022, 3, 25, 15, 14, 5, 242, DateTimeKind.Local).AddTicks(8439));

            migrationBuilder.InsertData(
               table: "APIs",
               columns: new[] { "APIId", "APIName", "BaseUrl", "APIKey"},
               values: new object[] { 2, 3, "not applicable", "not applicable"});

            migrationBuilder.InsertData(
                table: "Assets",
                columns: new[] { "AssetId", "APIId", "AssetType", "ISN", "Name", "SymbolForApi", "UpdatedOn", "Value" },
                values: new object[] { 4, 2, 5, "not applicable", "Groepsverzekering IS", "not applicable", new DateTime(2022, 3, 25, 15, 14, 5, 242, DateTimeKind.Local).AddTicks(8442), 0m });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 4);

            migrationBuilder.UpdateData(
                table: "APIs",
                keyColumn: "APIId",
                keyValue: 1,
                column: "APIName",
                value: 1);

            migrationBuilder.UpdateData(
                table: "APIs",
                keyColumn: "APIId",
                keyValue: 2,
                column: "APIName",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 1,
                column: "UpdatedOn",
                value: new DateTime(2022, 3, 25, 15, 6, 42, 180, DateTimeKind.Local).AddTicks(1129));

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 2,
                column: "UpdatedOn",
                value: new DateTime(2022, 3, 25, 15, 6, 42, 180, DateTimeKind.Local).AddTicks(1180));

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 3,
                column: "UpdatedOn",
                value: new DateTime(2022, 3, 25, 15, 6, 42, 180, DateTimeKind.Local).AddTicks(1183));
        }
    }
}
