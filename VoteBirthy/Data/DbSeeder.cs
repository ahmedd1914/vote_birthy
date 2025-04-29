using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoteBirthy.Models;

namespace VoteBirthy.Data
{
    public class DbSeeder
    {
        public static async Task SeedData(AppDbContext context)
        {
            if (!context.Employees.Any())
            {
                // Seed employees
                var employees = new List<Employee>
                {
                    new Employee
                    {
                        EmployeeId = Guid.NewGuid(),
                        Username = "john.doe",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                        FullName = "John Doe",
                        DateOfBirth = new DateTime(1990, 5, 15)
                    },
                    new Employee
                    {
                        EmployeeId = Guid.NewGuid(),
                        Username = "jane.smith",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                        FullName = "Jane Smith",
                        DateOfBirth = new DateTime(1992, 8, 22)
                    },
                    new Employee
                    {
                        EmployeeId = Guid.NewGuid(),
                        Username = "bob.johnson",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                        FullName = "Bob Johnson",
                        DateOfBirth = new DateTime(1985, 3, 10)
                    }
                };

                context.Employees.AddRange(employees);
            }

            if (!context.Gifts.Any())
            {
                // Seed gift catalog
                var gifts = new List<Gift>
                {
                    new Gift
                    {
                        GiftId = Guid.NewGuid(),
                        Name = "Coffee Mug",
                        Description = "A personalized coffee mug with the company logo."
                    },
                    new Gift
                    {
                        GiftId = Guid.NewGuid(),
                        Name = "Gift Card",
                        Description = "A $50 gift card to a popular online retailer."
                    },
                    new Gift
                    {
                        GiftId = Guid.NewGuid(),
                        Name = "Wireless Headphones",
                        Description = "High-quality wireless headphones for music and calls."
                    },
                    new Gift
                    {
                        GiftId = Guid.NewGuid(),
                        Name = "Power Bank",
                        Description = "A portable power bank for charging devices on the go."
                    },
                    new Gift
                    {
                        GiftId = Guid.NewGuid(),
                        Name = "Book Set",
                        Description = "A set of bestselling books in the recipient's favorite genre."
                    }
                };

                context.Gifts.AddRange(gifts);
            }

            await context.SaveChangesAsync();
        }
    }
} 