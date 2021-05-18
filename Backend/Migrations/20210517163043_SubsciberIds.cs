using Backend.Db;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Backend.Migrations
{
    public partial class SubsciberIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            using (var db = new BgDbContext())
            {
                foreach (var user in db.Users)
                    user.EmailUnsubscribeId = Guid.NewGuid();
                db.SaveChanges();
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
