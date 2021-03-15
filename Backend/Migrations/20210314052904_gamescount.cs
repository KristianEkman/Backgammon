using Microsoft.EntityFrameworkCore.Migrations;
using System;

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

            var sp = @"CREATE PROCEDURE [dbo].[GetRank]
                        (
                            @Id uniqueidentifier = null
                        )
                        AS
                        BEGIN
                            SET NOCOUNT ON
	                        ;WITH NumberedRows
	                        AS(SELECT ROW_NUMBER() OVER(ORDER BY Elo desc) AS Rank, id, elo
	                        FROM Users)
	                        select Rank from NumberedRows where id = @Id
                        END";

            migrationBuilder.Sql(sp);


            var insertGuest = $"INSERT INTO [dbo].[Users] ([Id],[Name],[Email],[PhotoUrl],[ProviderId],[SocialProvider],[Elo],[GameCount]) VALUES('{Guid.Empty}', 'Guest', '', '', '', '', 1200, 0)";
            migrationBuilder.Sql(insertGuest);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameCount",
                table: "Users");
        }
     }
}
