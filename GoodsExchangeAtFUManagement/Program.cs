using GoodsExchangeAtFUManagement.DAO;
using GoodsExchangeAtFUManagement.DataService;
using GoodsExchangeAtFUManagement.Hubs;
using GoodsExchangeAtFUManagement.Middlewares;
using GoodsExchangeAtFUManagement.Repository.Mappers;
using GoodsExchangeAtFUManagement.Repository.Repositories.CampusRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.CategoryRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.CoinPackRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.CoinTransactionRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.OTPCodeRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.PaymentRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.PostModeRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.ProductImagesRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.ProductPostRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.ProductTransactionRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.RefreshTokenRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.ReportRepositories;
using GoodsExchangeAtFUManagement.Repository.Repositories.UserRepositories;
using GoodsExchangeAtFUManagement.Service.Services.AuthenticationService;
using GoodsExchangeAtFUManagement.Service.Services.CampusServices;
using GoodsExchangeAtFUManagement.Service.Services.CategoryServices;
using GoodsExchangeAtFUManagement.Service.Services.CoinPackServices;
using GoodsExchangeAtFUManagement.Service.Services.CoinTransactionServices;
using GoodsExchangeAtFUManagement.Service.Services.EmailServices;
using GoodsExchangeAtFUManagement.Service.Services.OTPServices;
using GoodsExchangeAtFUManagement.Service.Services.PostModeServices;
using GoodsExchangeAtFUManagement.Service.Services.ProductPostServices;
using GoodsExchangeAtFUManagement.Service.Services.ProductTransactionServices;
using GoodsExchangeAtFUManagement.Service.Services.ReportServices;
using GoodsExchangeAtFUManagement.Service.Services.UserServices;
using GoodsExchangeAtFUManagement.Service.Services.VnPayServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSignalR();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//========================================== MAPPER ===============================================

builder.Services.AddAutoMapper(typeof(MapperProfile).Assembly);

//========================================== MIDDLEWARE ===========================================

builder.Services.AddSingleton<GlobalExceptionMiddleware>();
builder.Services.AddSingleton<SharedDB>();

//========================================== REPOSITORY ===========================================

builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IOTPCodeRepository, OTPCodeRepository>();
builder.Services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddTransient<ICampusRepository, CampusRepository>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<ICoinPackRepository, CoinPackRepository>();
builder.Services.AddTransient<IPostModeRepository, PostModeRepository>();
builder.Services.AddTransient<IProductPostRepository, ProductPostRepository>();
builder.Services.AddTransient<IProductImagesRepository, ProductImagesRepository>();
builder.Services.AddTransient<ICoinTransactionRepository, CoinTransactionRepository>();
builder.Services.AddTransient<IPaymentRepository, PaymentRepository>();
builder.Services.AddTransient<IProductTransactionRepository, ProductTransactionRepository>();
builder.Services.AddTransient<IReportRepository, ReportRepository>();


//=========================================== SERVICE =============================================

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOTPService, OTPService>();
builder.Services.AddScoped<ICampusService, CampusService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICoinPackService, CoinPackService>();
builder.Services.AddScoped<IPostModeService, PostModeService>();
builder.Services.AddScoped<IProductPostService, ProductPostService>();
builder.Services.AddScoped<ICoinTransactionService, CoinTransactionService>();
builder.Services.AddScoped<IProductTransactionService, ProductTransactionService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IVnPayService, VnPayService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

//=========================================== CORS ================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAll",
                      policy =>
                      {
                          policy
                          //.WithOrigins("http://localhost:3000")
                          .AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                          //.AllowCredentials();
                      });
});

//========================================== AUTHENTICATION =======================================

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = "FPTUStudents",
            ValidAudience = "FPTUStudents",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("b5aec850e7e188935e6832264e527945aef42149aa8567b64028f6b42decb66a")),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
        };
    });

//================================================ SWAGGER ========================================

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
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
app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/Chat");
app.Run();
