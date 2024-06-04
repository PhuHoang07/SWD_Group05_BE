using GoodsExchangeAtFUManagement.Middlewares;
using GoodsExchangeAtFUManagement.Repository.Mappers;
using GoodsExchangeAtFUManagement.Repository.Models;
using GoodsExchangeAtFUManagement.Repository.Repositories.UserRepositories;
using GoodsExchangeAtFUManagement.Repository.UnitOfWork;
using GoodsExchangeAtFUManagement.Service.Services.EmailServices;
using GoodsExchangeAtFUManagement.Service.Services.UserServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//=======================================CONNECTION STRING========================================

builder.Services.AddDbContext<GoodsExchangeAtFuContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

//==========================================MAPPER================================================

builder.Services.AddAutoMapper(typeof(MapperProfile).Assembly);

//==========================================MIDDLEWARE============================================

builder.Services.AddSingleton<GlobalExceptionMiddleware>();

//============================================REPO================================================

builder.Services.AddTransient<IUserRepository, UserRepository>();


//===========================================SERVICE==============================================

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserService, UserService>();


//================================================CORS============================================

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAll",
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                      });
});

//===========================================AUTHENTICATION========================================

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = "JWTIssuerSigningIssuerGEFUSWDG5",
            ValidAudience = "JWTIssuerSigningAudienceGEFUSWDG5",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("b5aec850e7e188935e6832264e527945aef42149aa8567b64028f6b42decb66a")),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
        };
    });

//===================================================SWAGGER=======================================

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
     {
         {
            new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
            new string[] {}
         }
     });
});

//===================================================================================================

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
