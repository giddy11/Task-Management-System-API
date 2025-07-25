using Microsoft.AspNetCore.Identity; // Needed for IPasswordHasher
using TaskManagement.Domain.UserManagement;
using TaskManagement.Persistence;

namespace TaskManagement.API;

public static class DatabaseSeeder
{
    public static void Seed(TaskManagementDbContext context, IPasswordHasher<User> passwordHasher)
    {
        if (context.Users.Any())
        {
            return;
        }

        var now = DateTime.UtcNow;
        var adminUser = CreateAdminUser(passwordHasher, now);

        context.Users.Add(adminUser);
        context.SaveChanges();
    }

    private static User CreateAdminUser(IPasswordHasher<User> passwordHasher, DateTime now)
    {
        var admin = new User
        {
            Email = "admin@taskmanager.com",
            FirstName = "Admin",
            LastName = "User",
            AccountType = AccountTypes.Admin,
            UserStatus = UserStatus.Active
        };

        const string defaultAdminPassword = "AdminPassword123!";

        admin.PasswordHash = passwordHasher.HashPassword(admin, defaultAdminPassword);

        return admin;
    }
}