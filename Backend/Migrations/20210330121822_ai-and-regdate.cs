using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend.Migrations
{
    public partial class aiandregdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Registered",
                table: "Users",
                type: "datetime2",
                nullable: true);

            var insertGuest = $"INSERT INTO [dbo].[Users] ([Id],[Name],[Email],[PhotoUrl],[ProviderId],[SocialProvider],[Elo],[GameCount]) VALUES('{Backend.Rules.Player.AiUser}', 'Aina', '', '', '', '', 1200, 0)";
            migrationBuilder.Sql(insertGuest);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Registered",
                table: "Users");
        }
    }
}
