
using Domain.Contracts;
using Domain.Entities.SecurityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Data;
using Services.Abstractions;
using Services;
using Services.Infrastructure;
using Shared.SecurityModels;
using AutoMapper;
using Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace Start
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly);
            
            // Add Identity services
            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<StoreContext>()
                .AddDefaultTokenProviders();
            
            // Add DbContext
            builder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
            });

            // Configure JWT Authentication
            var jwtOptions = builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };
            });
            
            // Add AutoMapper - scan all assemblies for profiles
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            
            // Add Unit of Work
            builder.Services.AddScoped<IUnitOFWork, UnitOfWork>();
            
            // Add File Storage Service (Cloudflare R2)
            builder.Services.AddSingleton<IFileStorageService, CloudflareR2FileStorage>();
            
            // Add Service Manager
            builder.Services.AddScoped<IServiceManager, ServiceManager>();
            
            // Add DbInitializer
            builder.Services.AddScoped<IDbInitializer, DbInitializer>();
            
            // Configure JWT Options
            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
            builder.Services.Configure<DomainSettings>(builder.Configuration.GetSection("DomainUrls"));
            
            // ============= CACHING CONFIGURATION =============
            // Add Response Caching
            builder.Services.AddResponseCaching();
            
            // Add Memory Cache for application-level caching
            builder.Services.AddMemoryCache();
            
            // ============= RATE LIMITING CONFIGURATION =============
            builder.Services.AddRateLimiter(options =>
            {
                // Default policy for general endpoints (100 requests per minute)
                options.AddFixedWindowLimiter("default", opt =>
                {
                    opt.PermitLimit = 100;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 5;
                });
                
                // Strict policy for authentication endpoints (5 requests per 15 minutes)
                options.AddFixedWindowLimiter("auth", opt =>
                {
                    opt.PermitLimit = 5;
                    opt.Window = TimeSpan.FromMinutes(15);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 0; // No queueing for auth - fail fast
                });
                
                // Moderate policy for product operations (200 requests per minute)
                options.AddFixedWindowLimiter("products", opt =>
                {
                    opt.PermitLimit = 200;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 10;
                });
                
                // Strict policy for file uploads (10 requests per 10 minutes)
                options.AddFixedWindowLimiter("uploads", opt =>
                {
                    opt.PermitLimit = 10;
                    opt.Window = TimeSpan.FromMinutes(10);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 2;
                });
                
                // Admin operations (50 requests per minute)
                options.AddFixedWindowLimiter("admin", opt =>
                {
                    opt.PermitLimit = 50;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 5;
                });
                
                // Handle rejected requests
                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        await context.HttpContext.Response.WriteAsJsonAsync(new
                        {
                            error = "Too many requests",
                            message = "Rate limit exceeded. Please try again later.",
                            retryAfter = retryAfter.TotalSeconds
                        }, cancellationToken);
                    }
                    else
                    {
                        await context.HttpContext.Response.WriteAsJsonAsync(new
                        {
                            error = "Too many requests",
                            message = "Rate limit exceeded. Please try again later."
                        }, cancellationToken);
                    }
                };
            });
                
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "E-Commerce API",
                    Version = "v1",
                    Description = "E-Commerce API with JWT Authentication"
                });

                // Add JWT Authentication to Swagger
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter 'Bearer' followed by a space and then your JWT token.\n\nExample: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();
            await InitializeDbAsync(app);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            
            // Enable Response Caching (must be before UseAuthentication)
            app.UseResponseCaching();
            
            // Enable Rate Limiting (must be after UseRouting if using routing)
            app.UseRateLimiter();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
            async Task InitializeDbAsync(WebApplication app)
            {
                using var scope = app.Services.CreateScope();
                var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                await dbInitializer.InitializeAsync();
                
                // Seed governorate shipping prices
                var storeContext = scope.ServiceProvider.GetRequiredService<StoreContext>();
                await GovernorateShippingSeeder.SeedGovernoratesAsync(storeContext);
            }
        }
    }
}
