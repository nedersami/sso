using SSO.Infrastructure.Data.Domain;
using SSO.Infrastructure.Business.Interface;
using SSO.Infrastructure.Business.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SSO.UI.Configurations
{
	public sealed class JwtTokenGenerator : IJwtTokenGenerator
	{
		private readonly SettingConfigurationModel settingConfiguration;

		public JwtTokenGenerator(SettingConfigurationModel settingConfiguration)
		{
			this.settingConfiguration = settingConfiguration;
		}

		public string GenerateAccessToken(ApplicationUser user, ApplicationRole role)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(settingConfiguration.SecretKey);
			var options = new IdentityOptions();
			var claims = CreateClaims(user, role);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddDays(7),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}

		private IEnumerable<Claim> CreateClaims(ApplicationUser user, ApplicationRole role)
		{
			var myClaims = new List<Claim>
			{
				new Claim("UserId", user.Id, ClaimValueTypes.String),
				new Claim(ClaimTypes.Email, user.Email, ClaimValueTypes.String),
				new Claim(ClaimTypes.Role, role.Name, ClaimValueTypes.String),
				new Claim(ClaimTypes.GivenName, user.Name, ClaimValueTypes.String),
				new Claim("RoleId", role.Id, ClaimValueTypes.String)
			};
			if (user.TenantId.HasValue)
				myClaims.Add(new Claim(ClaimTypes.UserData, user.TenantId.Value.ToString(), ClaimValueTypes.String));
			return myClaims;
		}
	}
}
