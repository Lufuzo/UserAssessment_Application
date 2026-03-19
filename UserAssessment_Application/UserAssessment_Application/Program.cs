using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using UserAssessment_Application.Data;
using UserAssessment_Application.Repositories.IRepo;
using UserAssessment_Application.Repositories.Repo;
using UserAssessment_Application.Services;
using UserAssessment_Application.Services.CoreServices;
using UserAssessment_Application.Services.ICoreServices;

var builder = WebApplication.CreateBuilder(args);


    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();


    // ADDING CORS HERE (BEFORE BUILD)
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngular",
            policy => policy
                .WithOrigins("http://localhost:4200")     
                .AllowAnyHeader()
                .AllowAnyMethod());
    });


// Database
//var connectionString = builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseNpgsql(builder.Configuration.GetConnectionString("AuthdbConnection")));
var connectionString = builder.Configuration.GetConnectionString("AuthdbConnection")
    ?? "Host=localhost;Port=5432;Database=authdb;Username=dbuser;Password=Passwordtest1";
//DOCKER
//var connectionString  = builder.Configuration.GetConnectionString("AuthdbConnection")
//    ?? "Server=postgres;Port=5432;Database=authdb;User Id=dbuser;Password=Passwordtest1;Timeout=30;CommandTimeout=60;Include Error Detail=true";
 


builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

var jwtSettings = builder.Configuration.GetSection("JWT");
var secretKey = jwtSettings["Secret"];
var key = System.Text.Encoding.ASCII.GetBytes(secretKey);


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),

        ValidateIssuer = false,
        ValidateAudience = false,

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

try
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
    }
}
catch { }

app.Run();