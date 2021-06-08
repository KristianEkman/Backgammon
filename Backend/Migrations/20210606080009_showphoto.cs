using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend.Migrations
{
    public partial class showphoto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShowPhoto",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowPhoto",
                table: "Users");
        }
    }
}
