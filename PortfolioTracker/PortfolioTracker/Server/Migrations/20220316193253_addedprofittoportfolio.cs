using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioTracker.Server.Migrations
{
    public partial class addedprofittoportfolio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>("Profit","Portfolio", nullable:false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 1,
                column: "UpdatedOn",
                value: new DateTime(2022, 3, 16, 20, 30, 11, 275, DateTimeKind.Local).AddTicks(2322));

            migrationBuilder.UpdateData(
                table: "Assets",
                keyColumn: "AssetId",
                keyValue: 2,
                column: "UpdatedOn",
                value: new DateTime(2022, 3, 16, 20, 30, 11, 275, DateTimeKind.Local).AddTicks(2364));
        }
    }
}
