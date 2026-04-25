using Domain.Contracts;
using Domain.Entities.PaymentEntities;
using Domain.Entities.ProductEntities;
using Domain.Entities.SecurityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Persistence
{
    public class DbInitializer : IDbInitializer
    {
        private readonly StoreContext _storeContext;

        private readonly UserManager<User> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(StoreContext storeContext,
          RoleManager<IdentityRole> roleManager,
          //NewModuleContext newModuleContext,
          UserManager<User> userManager)
        {
            _storeContext = storeContext;
            _roleManager = roleManager;
            _userManager = userManager;
            //_newModuleContext = newModuleContext;
        }

      

        public async Task InitializeAsync()
        {
            try
            {
                Console.WriteLine("Attempting to connect to database...");
                bool hasConnection = false;

                try
                {
                    // Test the connection before attempting any operations
                    hasConnection = await _storeContext.Database.CanConnectAsync();
                    Console.WriteLine($"Database connection test: {(hasConnection ? "Successful" : "Failed")}");

                    if (!hasConnection)
                    {
                        Console.WriteLine("Cannot connect to database. Check connection string and network.");
                        return;
                    }
                }
                catch (Exception connEx)
                {
                    Console.WriteLine($"Connection test error: {connEx.Message}");
                    Console.WriteLine($"Connection error type: {connEx.GetType().Name}");
                    if (connEx.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {connEx.InnerException.Message}");
                    }
                    return;
                }

                if (_storeContext.Database.GetPendingMigrations().Any())
                {
                    Console.WriteLine("Applying pending migrations...");
                    await _storeContext.Database.MigrateAsync();
                    Console.WriteLine("Migrations completed successfully");
                }

                // Initialize Identity (roles and admin user)
                await InitializeIdentityAsync();

                // Seed product data
                await SeedProductDataAsync();

               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization error: {ex.Message}");
                Console.WriteLine($"Error type: {ex.GetType().Name}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner exception type: {ex.InnerException.GetType().Name}");
                }
                // Re-throw only critical exceptions that should stop the application
                if (ex is DbUpdateException || ex is SqlException)
                    throw;
            }
        }

        private async Task SeedProductDataAsync()
        {
            try
            {
                // Get the seeding directory path
                string seedingPath = GetSeedingPath();
                Console.WriteLine($"Seeding path: {seedingPath}");

                // ProductSize and ProductColor are now enums, no seeding needed
                // ProductBrand and ProductType entities removed from simplified design

                // Seed Products
                if (!_storeContext.Products.Any())
                {
                    Console.WriteLine("Seeding products...");
                    string productsPath = Path.Combine(seedingPath, "products.json");
                    if (File.Exists(productsPath))
                    {
                        var productsData = await File.ReadAllTextAsync(productsPath);
                        var products = JsonSerializer.Deserialize<List<Product>>(productsData);
                        if (products != null && products.Any())
                        {
                            await _storeContext.Products.AddRangeAsync(products);
                            await _storeContext.SaveChangesAsync();
                            Console.WriteLine($"Added {products.Count} products");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Products file not found at: {productsPath}");
                    }
                }

                // Seed Payment Settings
                if (!_storeContext.PaymentSettings.Any())
                {
                    Console.WriteLine("Seeding payment settings...");
                    var paymentSettings = new List<PaymentSettings>
                    {
                        new PaymentSettings
                        {
                            Method = PaymentMethod.VodafoneCash,
                            PhoneNumber = "01142029061",
                            DisplayName = "Vodafone Cash",
                            IsActive = true
                        },
                        new PaymentSettings
                        {
                            Method = PaymentMethod.EtisalatCash,
                            PhoneNumber = "01142029061",
                            DisplayName = "Etisalat Cash",
                            IsActive = true
                        },
                        new PaymentSettings
                        {
                            Method = PaymentMethod.OrangeCash,
                            PhoneNumber = "01142029061",
                            DisplayName = "Orange Cash",
                            IsActive = true
                        },
                        new PaymentSettings
                        {
                            Method = PaymentMethod.InstaPay,
                            PhoneNumber = "01142029061",
                            DisplayName = "InstaPay",
                            IsActive = true
                        }
                    };
                    await _storeContext.PaymentSettings.AddRangeAsync(paymentSettings);
                    await _storeContext.SaveChangesAsync();
                    Console.WriteLine($"Added {paymentSettings.Count} payment settings");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding product data: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
        }

        private string GetSeedingPath()
        {
            // Try multiple possible paths
            var possiblePaths = new[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Persistence", "Data", "Seeding"),
                Path.Combine(Directory.GetCurrentDirectory(), "Persistence", "Data", "Seeding"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Seeding"),
                Path.Combine(Directory.GetCurrentDirectory(), "Data", "Seeding"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "Data", "Seeding"),
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Data", "Seeding")
            };

            foreach (var path in possiblePaths)
            {
                if (Directory.Exists(path))
                {
                    Console.WriteLine($"Found seeding directory at: {path}");
                    return path;
                }
            }

            // If no path found, return the default one
            var defaultPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Persistence", "Data", "Seeding");
            Console.WriteLine($"No seeding directory found, using default: {defaultPath}");
            return defaultPath;
        }

        public async Task InitializeIdentityAsync()
        {
            // Create all roles from the Role enum
            var roles = Enum.GetValues(typeof(Role)).Cast<Role>();
            
            foreach (var role in roles)
            {
                var roleName = role.ToString();
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                    Console.WriteLine($"Created role: {roleName}");
                }
            }

            if (!_userManager.Users.Any())
            {
                var adminUser = new User
                {
                    DisplayName = "Abdelrhman Ali",
                    FirstName="Abdelrhman",
                    LastName="Ali",
                    Email = "abdelrhmanali2119@gmail.com",
                    UserName = "AbdelrhmanAli22",
                    PhoneNumber = "01142029061"
                };

                await _userManager.CreateAsync(adminUser, "Abdo@888");
                await _userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}

