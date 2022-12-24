using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MigrationBundleConsoleAppExample.Migrations
{
    public partial class Fourth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE [dbo].[Customers]
                (
                    CustomerID  nchar(5)      NOT NULL,
                    CompanyName nvarchar(50)  NOT NULL,
                    ContactName nvarchar (50) NULL,
                    Phone       nvarchar (24) NULL,
                    CONSTRAINT [PK_Customers] PRIMARY KEY ([CustomerID])
                )
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP TABLE [dbo].[Customers]
            ");
        }
    }
}
