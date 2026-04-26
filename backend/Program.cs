using Microsoft.EntityFrameworkCore;
using TaskApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy =
        System.Text.Json.JsonNamingPolicy.SnakeCaseLower);

builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlite("Data Source=taskapi.db"));

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseCors();
app.MapControllers();
app.Run();
