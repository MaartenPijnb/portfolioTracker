using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioTracker.Server.Migrations
{
    public partial class BrokerTypeToTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BrokerType",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrokerType",
                table: "Transactions");

            
        }
    }
}
