using System.Text;
using api.Data;
using api.Interfaces;
using api.Models;
using api.Repository;
using api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

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
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers()
  .AddNewtonsoftJson(options => {
    options.SerializerSettings.ReferenceLoopHandling
      = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
  });
builder.Services.AddDbContext<AppDbContext>(options => {
  options.UseMySql(
    connectionString,
    ServerVersion.AutoDetect(connectionString),
    mySqlOptionBuilder => { }
  );
});
builder.Services.AddIdentity<AppUser, IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 12;
  })
  .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme =
      options.DefaultChallengeScheme =
        options.DefaultForbidScheme =
          options.DefaultScheme =
            options.DefaultSignInScheme =
              options.DefaultSignOutScheme
                = JwtBearerDefaults.AuthenticationScheme;
  })
  .AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
      ValidateIssuer = true,
      ValidIssuer = builder.Configuration["Jwt:Issuer"],
      ValidateAudience = true,
      ValidAudience = builder.Configuration["Jwt:Audience"],
      IssuerSigningKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SigningKey"])
      ),
    };
  });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options => {
  options.SwaggerDoc("v1", new OpenApiInfo {
    Title = "learn aspnetcore web API",
    Version = "v1",
  });
  options.AddSecurityDefinition(
    "Bearer",
    new OpenApiSecurityScheme {
      In = ParameterLocation.Header,
      Description = "Please insert JWT token",
      Name = "Authorization",
      Type = SecuritySchemeType.Http,
      BearerFormat = "JWT",
      Scheme = "Bearer",
    });
  options.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
      new OpenApiSecurityScheme {
        Reference = new OpenApiReference {
          Type = ReferenceType.SecurityScheme,
          Id = "Bearer",
        },
      },
      []
    },
  });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();