using DAL.Database;
using Entity.TableModel;
using Entity.TableModel.Membership;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace DAL.Extension
{
    public static class SeedDatabaseUser
    {
        public static IApplicationBuilder SeedMembership(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                string roleName = "SuperAdmin";
                string email = "info@kara.com";
                string userName = "Kamil";
                string password = "Kamil_2025!@0_";

                try
                {
                    // 1. DATABASE READY Mİ KONTROL ET
                    if (!dbContext.Database.CanConnect())
                    {
                        Console.WriteLine("Database bağlantısı kurulamadı.");
                        return app;
                    }

                    // 2. ROLLERİ OLUŞTUR
                    var roles = new[] { "SuperAdmin", "Admin", "User", "Moderator" };

                    foreach (var role in roles)
                    {
                        if (!roleManager.RoleExistsAsync(role).Result)
                        {
                            var roleResult = roleManager.CreateAsync(new AppRole { Name = role }).Result;
                            Console.WriteLine(roleResult.Succeeded ?
                                $"{role} rolü oluşturuldu." :
                                $"{role} rolü oluşturulamadı: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                        }
                    }

                    // 3. SUPERADMIN KULLANICISI OLUŞTUR (APPUSER)
                    AppUser appUser = userManager.FindByEmailAsync(email).Result;
                    if (appUser == null)
                    {
                        appUser = new AppUser
                        {
                            FirstName = "Mirfarid",
                            LastName = "Aghalarov",
                            UserName = userName,
                            Email = email,
                            EmailConfirmed = true,
                            PhoneNumberConfirmed = true,
                            TwoFactorEnabled = false,
                            LockoutEnabled = false,
                            AccessFailedCount = 0
                        };

                        var userResult = userManager.CreateAsync(appUser, password).Result;
                        if (!userResult.Succeeded)
                        {
                            Console.WriteLine($"AppUser oluşturulamadı: {string.Join(", ", userResult.Errors.Select(e => e.Description))}");
                            return app;
                        }
                        Console.WriteLine($"AppUser oluşturuldu: {userName}");
                    }
                    else
                    {
                        Console.WriteLine($"AppUser zaten mevcut: {email}");
                    }

                    // 4. SUPERADMIN ROLÜNÜ ATA
                    if (!userManager.IsInRoleAsync(appUser, roleName).Result)
                    {
                        var roleResult = userManager.AddToRoleAsync(appUser, roleName).Result;
                        Console.WriteLine(roleResult.Succeeded ?
                            $"Rol atandı: {userName} -> {roleName}" :
                            $"Rol atanamadı: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    }

                    // 5. USER TABLOSUNA KAYIT EKLE
                    var existingUser = dbContext.ApplicationUsers
                        .FirstOrDefault(u => u.Email == email || u.AppUserId == appUser.Id);

                    if (existingUser == null)
                    {
                        var user = new User
                        {
                            Name = $"{appUser.FirstName} {appUser.LastName}".Trim(),
                            Username = appUser.UserName,
                            Email = appUser.Email,
                            AppUserId = appUser.Id,
                            Avatar = null,
                            Bio = "Sistem yöneticisi",
                            IsOnline = true,
                            LastSeen = DateTime.UtcNow,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            Deleted = 0
                        };

                        dbContext.ApplicationUsers.Add(user);
                        dbContext.SaveChanges();
                        Console.WriteLine($"User tablosuna eklendi: {user.Name} (ID: {user.Id})");
                    }
                    else
                    {
                        // AppUserId'yi güncelle
                        if (existingUser.AppUserId == null || existingUser.AppUserId != appUser.Id)
                        {
                            existingUser.AppUserId = appUser.Id;
                            existingUser.UpdatedAt = DateTime.UtcNow;
                            dbContext.SaveChanges();
                            Console.WriteLine($"User güncellendi: AppUserId={appUser.Id}");
                        }
                        Console.WriteLine($"User zaten mevcut: {existingUser.Name}");
                    }

                    // 6. TEST KULLANICILARI OLUŞTUR (Opsiyonel)
                    CreateTestUsers(dbContext, userManager);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Seed işleminde hata: {ex.Message}");
                    Console.WriteLine($"StackTrace: {ex.StackTrace}");
                }
            }

            return app;
        }

        private static void CreateTestUsers(ApplicationDbContext dbContext, UserManager<AppUser> userManager)
        {
            try
            {
                var testUsers = new[]
                {
                    new { FirstName = "Ali", LastName = "Yılmaz", UserName = "aliyilmaz", Email = "ali@test.com", Password = "Test_123!" },
                    new { FirstName = "Ayşe", LastName = "Kaya", UserName = "aysekaya", Email = "ayse@test.com", Password = "Test_123!" },
                    new { FirstName = "Mehmet", LastName = "Demir", UserName = "mehmetdemir", Email = "mehmet@test.com", Password = "Test_123!" }
                };

                foreach (var testUser in testUsers)
                {
                    // AppUser oluştur
                    var appUser = userManager.FindByEmailAsync(testUser.Email).Result;
                    if (appUser == null)
                    {
                        appUser = new AppUser
                        {
                            FirstName = testUser.FirstName,
                            LastName = testUser.LastName,
                            UserName = testUser.UserName,
                            Email = testUser.Email,
                            EmailConfirmed = true
                        };

                        var result = userManager.CreateAsync(appUser, testUser.Password).Result;
                        if (result.Succeeded)
                        {
                            // User tablosuna ekle
                            var user = new User
                            {
                                Name = $"{testUser.FirstName} {testUser.LastName}",
                                Username = testUser.UserName,
                                Email = testUser.Email,
                                AppUserId = appUser.Id,
                                IsOnline = false,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow,
                                Deleted = 0
                            };

                            dbContext.ApplicationUsers.Add(user);
                        }
                    }
                }

                dbContext.SaveChanges();
                Console.WriteLine("Test kullanıcıları oluşturuldu.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test kullanıcıları oluşturulurken hata: {ex.Message}");
            }
        }
    }
}