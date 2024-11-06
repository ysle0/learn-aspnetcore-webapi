using api.Data;
using api.Interfaces;
using api.Models;
using api.Repository;
using api.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string? connectionString = builder
  .Configuration
  .GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString)) {
  throw new Exception("Connection string not found");
}

// Inject dependencies
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers()
  .AddNewtonsoftJson(options => {
    options.SerializerSettings.ReferenceLoopHandling
      = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
  });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options => {
  options.UseMySql(
    connectionString,
    ServerVersion.AutoDetect(connectionString),
    mySqlOptionBuilder => { }
  );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

await app.RunAsync();