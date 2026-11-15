using E_commerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace E_commerce.Services
{
    public class AppUserClaimsPrincipalFactory
        : UserClaimsPrincipalFactory<AppUsuario, IdentityRole>
    {
        public AppUserClaimsPrincipalFactory(
            UserManager<AppUsuario> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> options)
            : base(userManager, roleManager, options) { }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AppUsuario user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("Nombre", user.Nombre ?? user.UserName ?? ""));
            return identity;
        }
    }
}