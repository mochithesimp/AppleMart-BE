using iPhoneBE.API.Middlewares;
using iPhoneBE.Data;
using iPhoneBE.Data.Data;
using iPhoneBE.Data.Mapping;
using iPhoneBE.Data.Model;
using iPhoneBE.Service;
using iPhoneBE.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using iPhoneBE.API.Hubs;

namespace iPhoneBE.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddSignalR();

            // Swagger Configuration
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                         new string[]{}
                     }
                 });
            });

            //identity
            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<AppleMartDBContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<DataProtectorTokenProvider<User>>("REFRESHTOKENPROVIDER");

            //add jwt
            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidIssuer = builder.Configuration["JWT:Issuer"],
                        ValidAudience = builder.Configuration["JWT:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]))
                    };

                    x.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine($"OnAuthenticationFailed: {context.Exception.Message}");
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            var accountService = context.HttpContext.RequestServices.GetRequiredService<IAccountServices>();
                            return accountService.ValidateToken(context);
                        },
                        OnChallenge = context =>
                        {
                            Console.WriteLine($"OnChallenge: {context.Error}, {context.ErrorDescription}");
                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddAuthorization();

            var server = builder.Configuration["server"] ?? "localhost";
            var database = builder.Configuration["database"] ?? "AppleMartDB";
            var port = builder.Configuration["port"] ?? "1433";
            var password = builder.Configuration["password"] ?? "12345";
            var user = builder.Configuration["dbuser"] ?? "sa";

            var connectionString = $"Server={server},{port};Database={database};User Id={user};Password={password};TrustServerCertificate=True;";

            Console.WriteLine("Connection String: " + builder.Configuration.GetConnectionString("DefaultConnection"));
            builder.Services.AddDbContext<AppleMartDBContext>(options =>
                options.UseSqlServer(connectionString)
                );

            builder.Services
                .AddRepository(builder.Configuration)
                .AddServices();

            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("http://localhost:3000") 
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials(); 
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseCors();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<ChatHub>("/chatHub");

            app.MapControllers();

            var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppleMartDBContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "A problem occurred during migration");
            }

            app.Run();
            //tao test
        }
    }
}
