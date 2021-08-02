using AutoMapper;
using SSO.Infrastructure.Data.Domain;
using SSO.Infrastructure.Business.Interface;
using SSO.Infrastructure.Business.Model;
using SSO.Infrastructure.Business.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SSO.UI.Controllers
{
	public class AccountController : Controller
	{
		#region [ Properties ]

		private SettingConfigurationModel _settingConfiguration;
		private IHttpContextAccessor httpContextAccessor;
		private AccountService _accountService;
		private readonly ILogger logger;
		private readonly IMapper mapper;

		#endregion

		#region [ Constructor ]

		public AccountController(
			IJwtTokenGenerator jwtTokenGenerator,
			SignInManager<ApplicationUser> signInManager,
			UserManager<ApplicationUser> userManager,
			RoleManager<ApplicationRole> roleManager,
			SettingConfigurationModel settingConfiguration,
			IHttpContextAccessor httpContextAccessor,
			ILogger<AccountController> logger,
			IMapper mapper
			) : base()
		{
			_accountService = new AccountService(jwtTokenGenerator, signInManager, userManager, roleManager, settingConfiguration, mapper);
			this.httpContextAccessor = httpContextAccessor;
			_settingConfiguration = settingConfiguration;
			this.logger = logger;
			this.mapper = mapper;
		}

		#endregion

		#region [ Login ]

		public ViewResult Login(string returnUrl)
		{
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> Login(LoginModel model, string returnUrl)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var user = await _accountService.Login(model, true);

					ViewData["UserName"] = user.Name;
					logger.LogInformation("Usuário logado: {0}", user.Name);

					if (string.IsNullOrEmpty(returnUrl))
						return RedirectToAction("Index", "Home");
					else
						return LocalRedirect(returnUrl);
				}
				catch (Exception ex)
				{
					logger.LogError("AccountController(Login) => {0}", ex.ToString());
					ModelState.AddModelError(string.Empty, "Login e/ou Senha inválidos");
				}
			}
			else
			{
				ModelState.AddModelError(string.Empty, "Login e/ou Senha inválidos");
			}

			return View(model);
		}

		private void PrepararIdentity(IdentityUserModel user, out ClaimsPrincipal claimsPrincipal, out AuthenticationProperties authProps)
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

			var userIdentity = new ClaimsIdentity(_settingConfiguration.ApplicationName);
			userIdentity.AddClaims(claims);
			claimsPrincipal = new ClaimsPrincipal(userIdentity);

			authProps = new AuthenticationProperties()
			{
				AllowRefresh = true,
				ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
				IsPersistent = true,
				IssuedUtc = DateTime.Now
			};
		}

		public async Task<ActionResult> ConfirmEmail(string userId, string code)
		{
			if (userId == null || code == null)
				return RedirectToAction("Login");

			var retorno = await _accountService.ConfirmEmail(userId, code);
			if (retorno == null)
			{
				var urlLogin = Url.Action("Login", "Account");
				retorno = $"Seu usuário foi ativado com sucesso!<br /><br />Clique <a href='{urlLogin}'>aqui</a> para se logar.";
			}
			ViewBag.Retorno = retorno;
			return View();
		}

		#endregion

		#region [ Acesso Negado ]

		public IActionResult AccessDenied()
		{
			ViewBag.Message = "Você não possui acesso a página que tentou acessar, por gentileza entre em contato com o Administrador.";
			ViewBag.MessageType = "danger";
			return View();
		}

		#endregion

		#region [ Logoff ]

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Logoff()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			await _accountService.Logout();
			return RedirectToAction("Index", "Home");
		}

		#endregion

		#region [ Password Remember ]

		public ViewResult ForgotPassword()
		{
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> ForgotPassword(ForgotPasswordModel model)
		{
			if (ModelState.IsValid)
			{
				ViewBag.Message = "Ocorreu uma falha na sua solicitação.";
				ViewBag.MessageType = "danger";
				if (await _accountService.SendPasswordResetLink(model.Email, _settingConfiguration))
				{
					ViewBag.Message = "Foi enviado para seu email um link para gerar sua nova senha!";
					ViewBag.MessageType = "success";
				}
			}
			return View();
		}

		public ViewResult ResetPassword(string token)
		{
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> ResetPassword(ChangeResetPasswordModel model)
		{
			ViewBag.Message = "Ocorreu uma falha na sua solicitação. O token para reiniciar a senha está expirado ou é inválido.";
			ViewBag.MessageType = "danger";
			if (ModelState.IsValid)
			{
				if (await _accountService
					.ResetPassword(new IdentityUserModel
					{
						Email = model.Email,
						Token = model.Token,
						PasswordHash = model.Senha
					}))
				{
					var urlLogin = Url.Action("Login", "Account");
					ViewBag.Message = $"Senha alterada com sucesso!<br /><br />Clique <a href='{urlLogin}'>aqui</a> para se logar.";
					ViewBag.MessageType = "success";
				}
			}
			return View();
		}

		[Authorize]
		public ViewResult ChangePassword()
		{
			return View();
		}

		[HttpPost]
		[Authorize]
		public async Task<JsonResult> ChangePassword(ChangePasswordModel model)
		{
			ViewBag.Message = "Ocorreu uma falha na sua solicitação.";
			ViewBag.MessageType = "danger";
			var result = false;
			var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
			var user = _accountService.Login(new LoginModel() { UserName = email, Password = model.SenhaAtual }).Result;
			if (user != null)
			{
				result = await _accountService.ChangePassword(new IdentityUserModel
				{
					Email = email,
					Token = model.SenhaAtual,
					PasswordHash = model.Senha
				});
				if (result)
				{
					ViewBag.Message = "Senha alterada com sucesso!";
					ViewBag.MessageType = "success";
				}
			}
			else
			{
				ViewBag.Message = "Senha Atual incorreta, por gentileza informe novamente.";
				ViewBag.MessageType = "danger";
			}
			return Json(new { ok = result, mensagem = ViewBag.Message, tipo = ViewBag.MessageType });
		}

		#endregion
	}
}
