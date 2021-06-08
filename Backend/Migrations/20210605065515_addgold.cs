using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend.Migrations
{
    public partial class addgold : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Users Set gold = 200");
            migrationBuilder.Sql("UPDATE Users Set gold = 1500 Where Id = 'ECC9A1FC-3E5C-45E6-BCE3-7C24DFE82C98'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
