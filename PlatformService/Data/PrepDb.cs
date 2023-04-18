using System;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;

namespace PlatformService.Data{
    public static class PrepDb{
        public static void PrepPopulation(IApplicationBuilder app, bool isProd){
            using(var serviceScope = app.ApplicationServices.CreateScope()){
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProd);
            }
        }

        private static void SeedData(AppDbContext context, bool isProd){
			
			if(isProd){
				Console.WriteLine("--> Attempting to apply migrations...");
				try{
					context.Database.Migrate();
				}
				catch(Exception ex){
					Console.WriteLine($"--> Could not run migrations: {ex.Message}");
				}
			}
			
            if(!context.Platforms.Any()){
                Console.WriteLine("--> Seeding data");

                context.Platforms.AddRange(
                    new Platform(){Name="Dot Net", Publisher="Microsoft", Cost="Free"},
                    new Platform(){Name="SQL Server Express", Publisher="Microsoft", Cost="Free"},
                    new Platform(){Name="Kubernetes", Publisher="Cloud Native Computing Foundation", Cost="Free"}
                );

                context.SaveChanges();

            }else{
                Console.WriteLine("--> We already have data");
            }

            if(!context.Users.Any()){
                Console.WriteLine("--> Seeding user data");

                    var hmac = new HMACSHA512();
                    var passwordSalt = hmac.Key;
                    var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes("123"));
                    
                    context.Users.AddRange(
                        new User(){Username = "Maho", PasswordHash = passwordHash, PasswordSalt = passwordSalt}
                    );

                context.SaveChanges();
            }else{
                Console.WriteLine("--> We already have user data");
            }

        }
    }
}