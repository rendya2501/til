using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MigrationBundleConsoleAppExample.Migrations
{
    public partial class Third : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE VUEW [dbo].[MyView]
                AS
                SELECT ID,Name FROM dbo.People
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW MyView");
        }
    }
}
