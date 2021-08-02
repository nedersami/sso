using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using SSO.API.Configurations;
using SSO.API.Helpers;
using SSO.Infrastructure.Data.Domain;
using SSO.Infrastructure.Business.Interface;
using SSO.Infrastructure.Business.Model;
using SSO.Infrastructure.Business.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SSO.API.Controllers
{
	[ApiController]
	public class AccountController : BaseController
	{
		private SettingConfigurationModel _settingConfiguration;
		private AccountService _accountService;
		private readonly ILogger _logger;

		public AccountController(
			IJwtTokenGenerator jwtTokenGenerator,
			SignInManager<ApplicationUser> signInManager,
			UserManager<ApplicationUser> userManager,
			RoleManager<ApplicationRole> roleManager,
			SettingConfigurationModel settingConfiguration,
			ILoggerFactory loggerFactory,
			IMapper mapper)
		{
			_accountService = new AccountService(jwtTokenGenerator, signInManager, userManager, roleManager, settingConfiguration, mapper);
			_logger = loggerFactory.CreateLogger<AccountController>();
			_settingConfiguration = settingConfiguration;
		}

		/// <summary>
		/// Responsável pela autenticação do Usuário
		/// </summary>
		/// <param name="model"></param>
		[HttpPost]
		[AllowAnonymous]
		[Route("Login")]
		[ProducesResponseType(typeof(IdentityUserModel), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(BadRequestResponseModel), (int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> Login(LoginModel model)
		{
			try
			{
				return Response(await _accountService.Login(model));
			}
			catch (BusinessException ex)
			{
				NotifyError(ex);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.ToString());
				NotifyError(ex);
			}
			return Response(null);
		}

		/// <summary>
		/// Responsável pela geração do token e envio do email para reset de senha
		/// </summary>
		/// <param name="email"></param>
		[HttpGet]
		[AllowAnonymous]
		[Route("ForgotPassword/{email}")]
		[ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(BadRequestResponseModel), (int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> SendPasswordResetLink(string email)
		{
			var retorno = false;

			try
			{
				retorno = await _accountService.SendPasswordResetLink(email, _settingConfiguration);
			}
			catch (BusinessException ex)
			{
				NotifyError(ex);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.ToString());
				NotifyError(ex);
			}

			return Response(retorno);
		}

		/// <summary>
		/// Responsável pela gravação da nova senha
		/// </summary>
		/// <param name="model"></param>
		[HttpPost]
		[AllowAnonymous]
		[Route("ResetPassword")]
		[ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(BadRequestResponseModel), (int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> ResetPassword(IdentityUserModel model)
		{
			var retorno = false;

			try
			{
				retorno = await _accountService.ResetPassword(model);
			}
			catch (BusinessException ex)
			{
				NotifyError(ex);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.ToString());
				NotifyError(ex);
			}

			return Response(retorno);
		}

		/// <summary>
		/// Responsável pela alteração da senha atual
		/// </summary>
		/// <param name="model"></param>
		[HttpPost]
		[AllowAnonymous]
		[Route("ChangePassword")]
		[ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(BadRequestResponseModel), (int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> ChangePassword(IdentityUserModel model)
		{
			var retorno = false;

			try
			{
				retorno = await _accountService.ChangePassword(model);
			}
			catch (BusinessException ex)
			{
				NotifyError(ex);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.ToString());
				NotifyError(ex);
			}

			return Response(retorno);
		}
	}
}