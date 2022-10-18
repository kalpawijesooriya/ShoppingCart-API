using BusinessLayer;
using DataAccessLayer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Text;
using WebApi;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Authentication;

var builder = WebApplication.CreateBuilder(args);

//Update the JWT config from the settings
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

// Add connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")?? "Data Source=ShoppingCart";

// Add a contect to services collection
builder.Services.AddDbContext<ShoppingDBContext>(x => x.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddControllers();

// Add auto mapper profiles to the container
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();

//Geting seecret from the config
var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:Secrect"]);

var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = false, //ToDo Upddate
    ValidateAudience = false, //ToDo Upddate
    RequireExpirationTime = false,
    ValidateLifetime = true
};
builder.Services.AddSingleton(tokenValidationParameters);

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwt =>
{
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = tokenValidationParameters;
});

//Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
.AddEntityFrameworkStores<ShoppingDBContext>();

builder.Services.AddScoped<IAuthHelper, AuthHelper>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUnitofWork, UnitofWork>();
builder.Services.AddScoped<IAuthService, AuthService>();

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
