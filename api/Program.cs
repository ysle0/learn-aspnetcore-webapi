using api.Data;
using api.Interfaces;
using api.Repository;
using api.Utils;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string? connectionString = builder
  .Configuration
  .GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString)) {
  throw new Exception("Connection string not found");
}

// init auto mapper
var autoMapper = AutoMapperWrapper.RegisterMappings();

// Inject dependencies
builder.Services.AddSingleton(autoMapper);
builder.Services.AddScoped<IStockRepository, StockRepository>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options => {
  // options.UseSqlServer(connectionString);
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

app.Run();