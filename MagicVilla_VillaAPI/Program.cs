using MagicVilla_VillaAPI;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Repository;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add database connectionstring
builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
});

builder.Services.AddScoped<IVillaRepository, VillaRepository>();
builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");

// We are using JWT (JSON web token) Authentication, other examples includes cookie-based authentication, OAuth Authentication

builder.Services.AddAuthentication(x =>
{
    //Used to specify the default authentication scheme used to authenticate a users credentials. In this case, we provide JwtBearerDefaults.AthenticationScheme is being used as the default 
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; 
    //Used to specify the default athentication scheme used to challenge a users credentials. In this case, it is also set to JwtBearerDefaults.AthenticationScheme
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x => { // by using AddJwtBearer we are configuring the JwtBearer middleware
    x.RequireHttpsMetadata = false; // this is set to true in production to ensure that the token is only sent over a secure https connection.
    x.SaveToken = true; // if you want to save the token to access it later in the request pipeline then set this to true
    x.TokenValidationParameters = new TokenValidationParameters // TokenValidationParameters is used to specify how the middleware should validate the JWT Token.
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false,
    };

});



builder.Services.AddAutoMapper(typeof(MappingConfig));
//builder.Services.AddControllers(/* option => option.ReturnHttpNotAcceptable = true*/).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();
builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => 
{
    //Swagger supports several types of security schemes, including basic authentication, OAuth2, and JWT bearer authentication. 
    //The AddSecurityDefinition method allows you to define a security scheme of your choice.
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme  
    { 
        Description = 
            "JWT Authorization header using the bearer scheme.\r\n\r\n " +
            "enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
            "Example: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement() 
    {
        { 
            new OpenApiSecurityScheme
            { 
                Reference = new OpenApiReference
                { 
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   
}

app.UseSwagger();
app.UseSwaggerUI();
//app.UseSwaggerUI( options => { 
//    options.RoutePrefix = String.Empty;
//});

app.UseHttpsRedirection();
//In order for the user to be authorized, the user has to be authenticated first
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
