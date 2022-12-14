using CIS341_Project.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CIS341_Project.Areas.Identity.Data
{
    public class InitializeUsersAndAdmin
    {
        private readonly static string AdministratorRole = "Admin";
        private readonly static string Password = "Password!1234567";
        private readonly static string AdminName = "admin@admin.com";
        private readonly static string[] Names = { "jimbo@jimbo.com", "tommy@tommy.com", "beth@beth.com" };

        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AuthenticationContext(serviceProvider.GetRequiredService<DbContextOptions<AuthenticationContext>>()))
            {
                context.Database.EnsureCreated();
                if (context.Users.Any())
                {
                    return;
                }
                var adminID = await EnsureUser(serviceProvider, Password, AdminName);
                await EnsureRole(serviceProvider, adminID, AdministratorRole);

                foreach (var name in Names)
                {
                    await EnsureUser(serviceProvider, Password, name);
                }
            }
        }

        // Check that user exists with provided email address --> create new user if none exists
        private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string userPw, string UserName)
        {
            // Access the UserManager service
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
            var familySchedulerContext = new FamilySchedulerContext(serviceProvider.GetRequiredService<DbContextOptions<FamilySchedulerContext>>());
            if (userManager != null)
            {
                // Find user by email address
                var user = await userManager.FindByNameAsync(UserName);
                if (user == null)
                {
                    // Create new user if none exists
                    if (familySchedulerContext != null)
                    {
                        user = new ApplicationUser { UserName = UserName, HouseholdMemberID = familySchedulerContext.HouseholdMembers.Where(h => h.Name == UserName).First().HouseholdMemberID };
                        await userManager.CreateAsync(user, userPw);
                    }
                    else
                        throw new Exception("familySchedulerContext null");
                }

                // Confirm the new user so that we can log in
                user.EmailConfirmed = true;
                await userManager.UpdateAsync(user);

                return user.Id;
            }
            else
                throw new Exception("userManager null");
        }

        // Check that role exists --> create new rule if none exists
        private static async Task EnsureRole(IServiceProvider serviceProvider, string uid, string role)
        {
            // Access RoleManager service
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager != null)
            {
                // Check whether role exists --> if not, create new role with the provided role name
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }

                // Retrieve user with the provided ID and add to the specified role
                var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
                if (userManager != null)
                {
                    var user = await userManager.FindByIdAsync(uid);
                    await userManager.AddToRoleAsync(user, role);
                }
                else
                    throw new Exception("userManager null");

            }
            else
                throw new Exception("roleManager null");
        }
    }
}