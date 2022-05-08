using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend.Migrations
{
    public partial class showallphotos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Update Users Set ShowPhoto = 1");
            migrationBuilder.Sql($"Update Users Set PhotoUrl = 'aina' Where Id ='{Backend.Rules.Player.AiUser}'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
