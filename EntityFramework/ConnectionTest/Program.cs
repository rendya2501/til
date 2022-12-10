using Microsoft.EntityFrameworkCore;


var inpurString = Console.ReadLine() ?? throw new ArgumentException("null");

var ob = new DbContextOptionsBuilder<DbContext>();
ob.UseSqlServer(inpurString);
using var dbContext = new DbContext(ob.Options);

var appliedMigratiosn = await dbContext.Database.GetAppliedMigrationsAsync();
foreach (var item in appliedMigratiosn)
    Console.WriteLine(item);
