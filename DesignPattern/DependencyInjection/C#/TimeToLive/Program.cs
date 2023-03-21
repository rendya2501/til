using TimeToLive.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigureServices(builder.Services);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// 検証用のサービスコンテナの登録
static void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<IMySingletonService, MySingletonService>();
    services.AddScoped<IMyScopedService, MyScopedService>();
    services.AddTransient<IMyTransientService, MyTransientService>();
}