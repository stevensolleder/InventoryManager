using System.Text;
using InventoryManager;
using InventoryManager_Backend;
using InventoryManager.Interfaces;
using InventoryManager.Options;
using InventoryManager.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;



WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IConfigurationSection databaseConfigurationSection = builder.Configuration.GetSection("Database");
DatabaseOptions databaseOptions = new DatabaseOptions(databaseConfigurationSection["ConnectionString"]!, new MariaDbServerVersion(databaseConfigurationSection["MariaDbServerVersion"]!));
builder.Services.AddSingleton(databaseOptions);
IConfigurationSection dataFilesConfigurationSection = builder.Configuration.GetSection("DataFiles");
DataFilesOptions dataFilesOptions = new DataFilesOptions(dataFilesConfigurationSection["Path"]!);
builder.Services.AddSingleton(dataFilesOptions);
IConfiguration clamConfigurationSection = builder.Configuration.GetSection("ClamAv");
ClamOptions clamOptions = new ClamOptions(clamConfigurationSection["Address"]!, uint.Parse(clamConfigurationSection["Port"]!));
builder.Services.AddSingleton(clamOptions);
IConfigurationSection jwtConfigurationSection = builder.Configuration.GetSection("Jwt");
JwtOptions jwtOptions = new JwtOptions(jwtConfigurationSection["Issuer"]!, jwtConfigurationSection["Audience"]!, new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfigurationSection["SigningKey"]!)), jwtConfigurationSection["SigningAlgorithm"]!,uint.Parse(jwtConfigurationSection["TokenExpirationTime"]!));
builder.Services.AddSingleton(jwtOptions);

builder.Services.AddControllers();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("x-api-version"),
        new MediaTypeApiVersionReader("x-api-version"));
});

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    IConfigurationSection configurationSection = builder.Configuration.GetSection("Database");
    options.UseMySql(databaseOptions.ConnectionString, databaseOptions.MariaDbServerVersion, 
        mySqlOptions => {
            mySqlOptions.EnableStringComparisonTranslations();
        });
});

/*builder.Services.AddIdentityCore<ApplicationUser>()
    .AddEntityFrameworkStores<DatabaseContext>();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 4;
    options.Password.RequiredUniqueChars = 0;
});*/

/*builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            IssuerSigningKey = Constants.SigningKey,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = Constants.Issuer,
            ValidateIssuer = true,
            ValidAudience = Constants.Audience,
            ValidateAudience = true
        };
    });*/

builder.Services.AddSingleton<IFileService, FileService>();
MimeTypes.FallbackMimeType = "application/octet-stream";

builder.Services.AddSwaggerGen(options =>
{
    options.SupportNonNullableReferenceTypes();
});



WebApplication app = builder.Build();

app.UseHttpsRedirection();

/*app.UseAuthentication();
app.UseAuthorization();*/

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();


//TODO Swagger
//TODO Authentication & Authorization