using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend.Migrations
{
    public partial class gamescount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameCount",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameCount",
                table: "Users");
        }
    }
}
