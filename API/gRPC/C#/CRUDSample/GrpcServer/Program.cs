using Dapper;
using GrpcCRUDSample;
using GrpcServer.Data.Repositories;
using GrpcServer.Services;
using Microsoft.Data.Sqlite;
using System.Data;


var connectionString = "Data Source=:memory:";
using var connection = new SqliteConnection(connectionString);
connection.Open();

// Create an in-memory database and initialize it.
await CreateDatabaseIfNotExists(connection);

var builder = WebApplication.CreateBuilder(args);

// // Configure in-memory connection string.
// builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
// {
//     { "ConnectionStrings:DefaultConnection", connectionString }
// });

// Configure services for the application.
ConfigureServices(builder.Services, connection);

var app = builder.Build();

// Configure the HTTP request pipeline.
Configure(app);

await app.RunAsync();

static async Task CreateDatabaseIfNotExists(IDbConnection connection)
{
    await connection.ExecuteAsync(
        @"CREATE TABLE IF NOT EXISTS Users
          (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            UniqueID TEXT
          );");

    var userCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Users");
    if (userCount == 0)
    {
        await connection.ExecuteAsync("INSERT INTO Users (Name,UniqueID) VALUES ('Alice','Sierra117'), ('Bob','BullDog'), ('Charlie','Cicago')");
    }
}

static void ConfigureServices(IServiceCollection services, IDbConnection connection)
{
    services.AddGrpc();

    // Configure database connection.
    services.AddSingleton(connection);

    // Register repositories.
    services.AddScoped<IUserRepository, UserRepository>();
}

static void Configure(WebApplication app)
{
    app.MapGrpcService<GreeterService>();
    app.MapGrpcService<UserService>();
    app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
}

//var connectionString = "Data Source=:memory:";
//using var connection = new SqliteConnection(connectionString);
//connection.Open();
//await CreateDatabaseIfNotExists(connection);


//var builder = WebApplication.CreateBuilder(args);
//builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
//{
//    { "ConnectionStrings:DefaultConnection", connectionString }
//});

//// Additional configuration is required to successfully run gRPC on macOS.
//// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

//// Add services to the container.
//builder.Services.AddGrpc();
//builder.Services.AddTransient<IDbConnection>(provider => new SqliteConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
//builder.Services.AddScoped<IUserRepository, UserRepository>();


//var app = builder.Build();

//// Configure the HTTP request pipeline.
//app.MapGrpcService<UserServiceImpl>();

//app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

//app.Run();

//static async Task CreateDatabaseIfNotExists(IDbConnection connection)
//{
//    await connection.ExecuteAsync(
//        @"CREATE TABLE IF NOT EXISTS Users
//          (
//            Id INTEGER PRIMARY KEY AUTOINCREMENT,
//            Name TEXT NOT NULL
//          );");

//    var userCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Users");
//    if (userCount == 0)
//    {
//        await connection.ExecuteAsync("INSERT INTO Users (Name) VALUES ('Alice'), ('Bob'), ('Charlie')");
//    }
//}