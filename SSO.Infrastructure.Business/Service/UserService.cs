using AutoMapper;
using SSO.Infrastructure.Data.Domain;
using SSO.Infrastructure.Business.Model;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Linq;
using System.Web;
using System.Net;
using System;
using Microsoft.EntityFrameworkCore;

namespace SSO.Infrastructure.Business.Service
{
	public class UserService
	{
		private UserManager<ApplicationUser> _userManager;
		private RoleManager<ApplicationRole> _roleManager;
		private readonly IMapper _mapper;

		public UserService(UserManager<ApplicationUser> userManager,
			RoleManager<ApplicationRole> roleManager,
			IMapper mapper)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_mapper = mapper;
		}

		public async Task<List<IdentityUserModel>> GetUsers()
		{
			var query = from e in _userManager.Users
						select new IdentityUserModel
						{
							Id = e.Id,
							Name = e.Name,
							Email = e.Email
						};

			var retorno = new List<IdentityUserModel>();

			var users = await _userManager.Users.ToListAsync();
			if (users != null && users.Count > 0)
			{
				foreach (var user in users)
				{
					var model = _mapper.Map<IdentityUserModel>(user);
					var roles = await _userManager.GetRolesAsync(user);
					model.Role = await _roleManager.FindByNameAsync(roles.FirstOrDefault());
					retorno.Add(model);
				}
			}

			return retorno;
		}

		public async Task<Dictionary<string, string>> GetRoles()
		{
			return await _roleManager.Roles.ToDictionaryAsync(r => r.Id, r => r.Name);
		}

		public async Task<bool> IsTenantRole(string roleId)
		{
			return await _roleManager.Roles.Where(x => x.Id == roleId).Select(x => x.IsTenantRole).FirstOrDefaultAsync();
		}

		public async Task<IdentityUserModel> GetExistingEmail(string email)
		{
			IdentityUserModel result = null;

			var user = await _userManager.FindByEmailAsync(email);
			if (user != null)
			{
				result = _mapper.Map<IdentityUserModel>(user);
			}
			return result;
		}

		public async Task<IdentityUserModel> Create(IdentityUserModel model, SettingConfigurationModel settingConfiguration)
		{
			IdentityUserModel userCreated = null;
			var result = await _userManager.CreateAsync(model, model.Token);
			if (result.Succeeded)
			{
				var domain = await _userManager.FindByNameAsync(model.UserName);
				var role = await _roleManager.FindByIdAsync(model.Role.Id);
				await _userManager.AddToRoleAsync(model, role.Name);
				userCreated = _mapper.Map<IdentityUserModel>(domain);
				userCreated.Role = role;

				var code = await _userManager.GenerateEmailConfirmationTokenAsync(domain);
				var callbackUrl = $"{ settingConfiguration.UrlDefaultApp }Account/ConfirmEmail/?userId={ domain.Id }&code={ HttpUtility.UrlEncode(code) }";

				var smtpClient = new SmtpClient
				{
					Host = settingConfiguration.SMTP_Host,
					Port = Convert.ToInt32(settingConfiguration.SMTP_Port),
					EnableSsl = true,
					Credentials = new NetworkCredential(settingConfiguration.SMTP_User, settingConfiguration.SMTP_Pass)
				};

				var from = new MailAddress(settingConfiguration.SMTP_From, settingConfiguration.SMTP_From_Alias);
				var to = new MailAddress(model.Email, model.Name);
				var message = new MailMessage(from, to)
				{
					Subject = $"{ settingConfiguration.ApplicationName } - Confirme seu Email",
					Body = $"Olá { model.Name },<br /><br />Por gentileza confirme seu usuário <a href='{ callbackUrl }'>clicando aqui</a>.",
					IsBodyHtml = true
				};

				using (message)
					await smtpClient.SendMailAsync(message);
			}
			return userCreated;
		}

		public async Task<IdentityUserModel> Get(string id)
		{
			IdentityUserModel result = null;
			var user = await _userManager.FindByIdAsync(id);
			if (user != null)
			{
				var roleName = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
				var role = await _roleManager.FindByNameAsync(roleName);
				result = _mapper.Map<IdentityUserModel>(user);
				result.Role = role;
			}
			return result;
		}

		public async Task<IdentityUserModel> Set(IdentityUserModel model)
		{
			IdentityUserModel result = null;
			var domain = await _userManager.FindByEmailAsync(model.UserName);
			domain.Name = model.Name;
			domain.PhoneNumber = model.PhoneNumber;
			domain.TenantId = model.TenantId;

			var updateResult = await _userManager.UpdateAsync(domain);
			if (updateResult.Succeeded)
			{
				var actualRole = (await _userManager.GetRolesAsync(domain)).FirstOrDefault();
				var newRole = await _roleManager.FindByIdAsync(model.Role.Id);

				if (actualRole != newRole.Name)
				{
					await _userManager.RemoveFromRoleAsync(model, actualRole);
					await _userManager.AddToRoleAsync(model, newRole.Name);
				}

				result = _mapper.Map<IdentityUserModel>(domain);
				result.Role = newRole;
			}
			return result;
		}

		public async Task Delete(string id)
		{
			var domain = await _userManager.FindByIdAsync(id);
			await _userManager.DeleteAsync(domain);
		}
	}
}
