using AutoMapper;
using SSO.Infrastructure.Data.Domain;
using SSO.Infrastructure.Business.Interface;
using SSO.Infrastructure.Business.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Web;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;

namespace SSO.Infrastructure.Business.Service
{
	public class AccountService
	{
		private readonly IJwtTokenGenerator _jwtTokenGenerator;
		private SignInManager<ApplicationUser> _signInManager;
		private UserManager<ApplicationUser> _userManager;
		private RoleManager<ApplicationRole> _roleManager;
		private readonly SettingConfigurationModel _settingConfiguration;
		private readonly IMapper _mapper;

		public AccountService(IJwtTokenGenerator jwtTokenGenerator,
			SignInManager<ApplicationUser> signInManager,
			UserManager<ApplicationUser> userManager,
			RoleManager<ApplicationRole> roleManager,
			SettingConfigurationModel settingConfiguration,
			IMapper mapper)
		{
			_jwtTokenGenerator = jwtTokenGenerator;
			_signInManager = signInManager;
			_userManager = userManager;
			_roleManager = roleManager;
			_settingConfiguration = settingConfiguration;
			_mapper = mapper;
		}

		public async Task<IdentityUserModel> Login(LoginModel model, bool signIn = false)
		{
			IdentityUserModel retorno = null;
			var domain = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, lockoutOnFailure: true);
			if (domain.Succeeded)
			{
				var user = await _userManager.FindByNameAsync(model.UserName);
				var roles = await _userManager.GetRolesAsync(user);

				retorno = _mapper.Map<IdentityUserModel>(user);
				retorno.Role = await _roleManager.FindByNameAsync(roles.FirstOrDefault());
				var accessToken = _jwtTokenGenerator.GenerateAccessToken(user, retorno.Role);
				retorno.Token = accessToken;

				if (signIn)
				{
					await PrepararIdentity(retorno, model.Rememberme);
				}
			}
			return retorno;
		}

		private async Task PrepararIdentity(IdentityUserModel user, bool IsPersistent)
		{
			ClaimsPrincipal claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
			if (claimsPrincipal?.Identity is ClaimsIdentity claimsIdentity)
			{
				var claims = new List<Claim>
				{
					new Claim("UserId", user.Id, ClaimValueTypes.String),
					new Claim(ClaimTypes.Email, user.Email, ClaimValueTypes.String),
					new Claim(ClaimTypes.Role, user.Role.Name, ClaimValueTypes.String),
					new Claim(ClaimTypes.GivenName, user.Name, ClaimValueTypes.String),
					new Claim("RoleId", user.Role.Id, ClaimValueTypes.String),
					new Claim("access_token", user.Token, ClaimValueTypes.String)
				};
				if (user.TenantId.HasValue)
					claims.Add(new Claim(ClaimTypes.UserData, user.TenantId.Value.ToString(), ClaimValueTypes.String));

				claimsIdentity.AddClaims(claims);
			}

			AuthenticationProperties authProps = new AuthenticationProperties()
			{
				AllowRefresh = true,
				ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
				IsPersistent = IsPersistent,
				IssuedUtc = DateTime.Now
			};

			await _signInManager.Context.SignInAsync(IdentityConstants.ApplicationScheme, claimsPrincipal, authProps);
		}

		public async Task<bool> SendPasswordResetLink(string email, SettingConfigurationModel settingConfiguration)
		{
			var retorno = false;
			var user = await _userManager.FindByNameAsync(email);
			var token = _userManager.GeneratePasswordResetTokenAsync(user);
			var encodedCode = HttpUtility.UrlEncode(token.Result);
			var resetLink = $"{ settingConfiguration.UrlDefaultApp }Account/ResetPassword/?token={ encodedCode }";

			var smtpClient = new SmtpClient
			{
				Host = settingConfiguration.SMTP_Host,
				Port = Convert.ToInt32(settingConfiguration.SMTP_Port),
				EnableSsl = true,
				Credentials = new NetworkCredential(settingConfiguration.SMTP_User, settingConfiguration.SMTP_Pass)
			};

			var from = new MailAddress(settingConfiguration.SMTP_From, settingConfiguration.SMTP_From_Alias);
			var to = new MailAddress(email, user.Name);
			var message = new MailMessage(from, to)
			{
				Subject = $"{ settingConfiguration.ApplicationName } - Nova Senha",
				Body = $"Olá { user.Name },<br /><br />Clique <a href='{ resetLink }'>aqui</a> para gerar uma nova senha de acesso do { settingConfiguration.ApplicationName }.",
				IsBodyHtml = true
			};

			using (message)
			{
				await smtpClient.SendMailAsync(message);
				retorno = true;
			}
			return retorno;
		}

		public async Task<bool> ResetPassword(IdentityUserModel model)
		{
			var user = await _userManager.FindByNameAsync(model.Email);
			var result = await _userManager.ResetPasswordAsync(user, model.Token, model.PasswordHash);
			return result.Succeeded;
		}

		public async Task<bool> ChangePassword(IdentityUserModel model)
		{
			var user = await _userManager.FindByNameAsync(model.Email);
			var result = await _userManager.ChangePasswordAsync(user, model.Token, model.PasswordHash);
			return result.Succeeded;
		}

		public async Task<string> ConfirmEmail(string userId, string code)
		{
			string result = null;
			var domain = await _userManager.FindByIdAsync(userId);
			if (domain == null)
			{
				result = string.Format(result, "Usuário não encontrado");
			}
			else
			{
				var resultConfirm = await _userManager.ConfirmEmailAsync(domain, code);
				if (!resultConfirm.Succeeded)
				{
					result = string.Join("<br />", resultConfirm.Errors);
				}
			}
			return result;
		}

		public async Task Logout()
		{
			await _signInManager.SignOutAsync();
		}
	}
}
