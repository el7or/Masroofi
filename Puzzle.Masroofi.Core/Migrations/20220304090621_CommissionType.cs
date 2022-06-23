using Microsoft.EntityFrameworkCore.Migrations;

namespace Puzzle.Masroofi.Core.Migrations
{
    public partial class CommissionType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommissionType",
                table: "CommissionSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommissionType",
                table: "CommissionSettings");
        }
    }
}
