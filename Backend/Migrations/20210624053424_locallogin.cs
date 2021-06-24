using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend.Migrations
{
    public partial class locallogin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LocalLogin",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocalLogin",
                table: "Users");
        }
    }
}
