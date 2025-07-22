using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Models.Const;
using Models.Domain;

namespace DataAcess.Seeds
{
	public class DefaultUsers
	{
		public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
		{
			var admin = new ApplicationUser()
			{
				Id = "63d0590f-2f52-4267-83c4-18d0499f9701",
				UserName = "admin",
				Email = "admin@handmade.com",
				FullName = "Admin",
				EmailConfirmed = true
			};

			var user = await userManager.FindByNameAsync(admin.UserName);

			if (user == null)
			{
				await userManager.CreateAsync(admin, "Admin@123");
				await userManager.AddToRoleAsync(admin, AppRoles.Admin);
				await userManager.AddToRoleAsync(admin, AppRoles.Seller);
				await userManager.AddToRoleAsync(admin, AppRoles.Customer);
			}

		}
	}
}
