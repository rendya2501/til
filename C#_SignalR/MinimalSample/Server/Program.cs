using Server.Hubs;

var builder = WebApplication.CreateBuilder(args);
// SignalRサービス追加
builder.Services.AddSignalR();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
// SignalR Hubの登録
app.MapHub<ChatHub>("/chatHub");

app.Run();
