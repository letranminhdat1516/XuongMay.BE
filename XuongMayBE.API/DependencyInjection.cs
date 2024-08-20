using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using XuongMay.Contract.Repositories.Entity;
using XuongMay.Contract.Services.Interface;
using XuongMay.Repositories.Context;
using XuongMay.Repositories.Entity;
using XuongMay.Services;
using XuongMay.Services.Service;
using XuongMay.Core.Base;
using Newtonsoft.Json;

namespace XuongMayBE.API
{
    public static class DependencyInjection
    {
        public static void AddConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigRoute();
            services.AddDatabase(configuration);
            services.AddJwtAuthentication(configuration);
            services.AddSwaggerGen();
            services.AddIdentity();
            services.AddInfrastructure(configuration);
            services.AddServices();
        }
        public static void ConfigRoute(this IServiceCollection services)
        {
            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });
        }

        public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseLazyLoadingProxies().UseSqlServer(configuration.GetConnectionString("MyCnn"));
            });
        }

        public static void AddIdentity(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
            })
             .AddEntityFrameworkStores<DatabaseContext>()
             .AddDefaultTokenProviders();
        }

        public static void AddSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(option =>
            {
                option.EnableAnnotations();
                option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme,
                    securityScheme: new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Description = "Enter Bearer Authorization: `Bearer Generated-JWT-Token`",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
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
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },new string[]{}
                    }

                });
            });
        }

        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("EditPolicy", policy =>
                {
                    policy.RequireClaim("Permission","CanEdit");
                });
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        var result = BaseResponse<string>.UnauthorizeResponse("Yêu cầu đăng nhập trước khi thực hiện");
                        return context.Response.WriteAsync(JsonConvert.SerializeObject(result));
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";
                        var result = BaseResponse<string>.ForbiddenResponse("Vai trò của bạn không thể truy cập vào tài nguyên này");
                        return context.Response.WriteAsync(JsonConvert.SerializeObject(result));
                    }
                };
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"]
                };
            });
        }

        public static void AddServices(this IServiceCollection services)
        {
            services
                .AddScoped<IUserService, UserService>();
            services
              .AddScoped<IOrderProductService, OrderProductService>();
            services
                .AddScoped<IOrderTaskService, OrderTaskService>();
            services
                .AddScoped<ICategoryService, CategoryService>();
            services
                .AddScoped<IProductService, ProductService>();
            services
                .AddScoped<IConveyorService, ConveyorService>();
        }
    }
}
