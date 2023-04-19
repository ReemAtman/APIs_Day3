using DAL_Poject.Data.Context;
using DAL_Poject.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Database

var connectionString = builder.Configuration.GetConnectionString("ProfileDB");
builder.Services.AddDbContext<ProfileDBContext>(options =>
    options.UseSqlServer(connectionString));

#endregion
#region Identity Services

builder.Services
    .AddIdentity<Student, IdentityRole>(options =>
    {
        options.Password.RequireUppercase = false;        
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 4;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ProfileDBContext>();

#endregion
#region Authentication 

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Cool";
    options.DefaultChallengeScheme = "Cool";
})
.AddJwtBearer("Cool", options =>
{
    var secretKeyString = builder.Configuration.GetValue<string>("SecretKey");
    var secretyKeyInBytes = Encoding.ASCII.GetBytes(secretKeyString ?? string.Empty);
    var secretKey = new SymmetricSecurityKey(secretyKeyInBytes);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = secretKey,
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

#endregion
#region Authorization

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminsAndUsers", policy => policy
        .RequireClaim(ClaimTypes.Role, "Admin", "User")
        .RequireClaim(ClaimTypes.NameIdentifier));

    options.AddPolicy("ManagersOnly", policy => policy
        .RequireClaim(ClaimTypes.Role, "Admin")
        .RequireClaim(ClaimTypes.NameIdentifier));
});

#endregion
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
