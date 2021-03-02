using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend.Migrations
{
    public partial class winner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Winner",
                table: "Games",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Winner",
                table: "Games");
        }
    }
}
