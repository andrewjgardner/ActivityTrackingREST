using ActivityTrackingAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.Converters.Add(new StringEnumConverter()));

builder.Services.AddDbContext<ActivityContext>(options => options.UseSqlite("Data Source=Data/SQLite/Activities.db"));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options => options.MapType<TimeSpan>(() => new OpenApiSchema { Type = "string", Example = new OpenApiString("00:30:00") }));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
