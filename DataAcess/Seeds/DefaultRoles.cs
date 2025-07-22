using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Models.Const;

namespace DataAcess.Seeds
{
	public static class DefaultRoles
	{
		public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
		{
			if (!roleManager.Roles.Any())
			{
				if (!roleManager.Roles.Any())
				{
					await roleManager.CreateAsync(new IdentityRole
					{
						Id = "e1b08b2c-3d7c-4cb1-a3a3-176d09ce1b6f",
						Name = AppRoles.Admin,
						NormalizedName = AppRoles.Admin.ToUpper()
					});

					await roleManager.CreateAsync(new IdentityRole
					{
						Id = "97f492ef-1f2e-4c01-9420-73b9b47a922a",
						Name = AppRoles.Seller,
						NormalizedName = AppRoles.Seller.ToUpper()
					});

					await roleManager.CreateAsync(new IdentityRole
					{
						Id = "6783f846-345c-4370-bc3e-d3d1221b7656",
						Name = AppRoles.Customer,
						NormalizedName = AppRoles.Customer.ToUpper()
					});
				}
			}
		}
	}
}
