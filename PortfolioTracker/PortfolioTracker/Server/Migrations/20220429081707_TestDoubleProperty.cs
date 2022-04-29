using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioTracker.Server.Migrations
{
    public partial class TestDoubleProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
   
            migrationBuilder.AddColumn<double>(
                name: "Test",
                table: "Transactions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Test",
                table: "Transactions");
        }
    }
}
