using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioTracker.Server.Migrations
{
    public partial class StaticDataAndTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssetId",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurencyType",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "APIs",
                columns: table => new
                {
                    APIId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    APIName = table.Column<int>(type: "int", nullable: false),
                    BaseUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    APIKey = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APIs", x => x.APIId);
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    AssetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ISN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SymbolForApi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    APIId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.AssetId);
                    table.ForeignKey(
                        name: "FK_Assets_APIs_APIId",
                        column: x => x.APIId,
                        principalTable: "APIs",
                        principalColumn: "APIId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "APIs",
                columns: new[] { "APIId", "APIKey", "APIName", "BaseUrl" },
                values: new object[] { 1, "6PoQCJV9O17jeUPS81UDN1sHJ86gKB4RahYraKSS", 0, "https://yfapi.net" });

            migrationBuilder.InsertData(
                table: "Assets",
                columns: new[] { "AssetId", "APIId", "ISN", "Name", "SymbolForApi", "UpdatedOn", "Value" },
                values: new object[] { 1, 1, "IE00B4L5Y983", "iShares Core MSCI World UCITS ETF USD (Acc)", "IWDA.AS", new DateTime(2022, 3, 6, 18, 47, 31, 299, DateTimeKind.Local).AddTicks(7619), 0m });

            migrationBuilder.InsertData(
                table: "Assets",
                columns: new[] { "AssetId", "APIId", "ISN", "Name", "SymbolForApi", "UpdatedOn", "Value" },
                values: new object[] { 2, 1, "IE00B4L5YC18", "iShares MSCI EM UCITS ETF USD (Acc)", "IEMA.AS", new DateTime(2022, 3, 6, 18, 47, 31, 299, DateTimeKind.Local).AddTicks(7652), 0m });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AssetId",
                table: "Transactions",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_APIId",
                table: "Assets",
                column: "APIId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Assets_AssetId",
                table: "Transactions",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "AssetId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Assets_AssetId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "APIs");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_AssetId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CurencyType",
                table: "Transactions");
        }
    }
}
