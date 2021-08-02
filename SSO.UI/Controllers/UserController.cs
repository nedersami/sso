using Helpers.DataTables.Interface;
using Helpers.DataTables;
using SSO.Infrastructure.Business.Service;
using SSO.Infrastructure.Data.Domain;
using SSO.Infrastructure.Business.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SSO.UI.Controllers
{
	[Authorize]
	public class UserController : Controller
	{
		#region [ Properties ]

		private SettingConfigurationModel _settingConfiguration;
		private UserService _userService;
		private readonly ILogger logger;
		private readonly IMapper mapper;

		#endregion

		#region [ Constructor ]

		public UserController(
			UserManager<ApplicationUser> userManager,
			RoleManager<ApplicationRole> roleManager,
			SettingConfigurationModel settingConfiguration,
			ILogger<AccountController> logger,
			IMapper mapper
			) : base()
		{
			_userService = new UserService(userManager, roleManager, mapper);
			_settingConfiguration = settingConfiguration;
			this.logger = logger;
			this.mapper = mapper;
		}

		#endregion

		private void ViewBagMenu()
		{
			ViewBag.MenuPai = "seguranca";
			ViewBag.Menu = "usuario";

			ViewBag.Titulo = "Usuários";
			ViewBag.Subtitulo = "Cadastros";
			ViewBag.Icone = "icon-user-tie";
		}

		public ViewResult Index(bool? mensagem)
		{
			ViewData["Title"] = "Usuários";

			ViewBag.Mensagem = "";
			if (mensagem.HasValue && mensagem.Value)
			{
				ViewBag.Mensagem = "Dados gravados com sucesso";
			}

			ViewBagMenu();
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> GetDataUserTable([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest tableQuery)
		{
			try
			{
				var users = await _userService.GetUsers();
				var query = users.AsQueryable();

				if (tableQuery.Search != null && !string.IsNullOrEmpty(tableQuery.Search.Value))
				{
					string filtro = tableQuery.Search.Value;
					query = query.Where(x => x.Name.Contains(filtro) ||
											 x.Email.Contains(filtro) ||
											 x.Role.Name.Contains(filtro));
				}

				int total = query.Count();
				query = query.AplicarPaginacaoOrdenacaoDataTable(tableQuery);
				var returnData = query.ToList();

				return Json(new DataTablesResponse(tableQuery.Draw, returnData, total, total));
			}
			catch (Exception ex)
			{
				logger.LogError("UserController(GetDataTableUser) => {0}", ex.ToString());
				throw ex;
			}
		}

		private async Task LoadViewBags()
		{
			var items = await _userService.GetRoles();
			var qItems = from e in items
						 select new SelectListItem
						 {
							 Text = e.Value,
							 Value = e.Key
						 };
			var listItems = qItems.ToList();
			ViewBag.RoleList = listItems;
			ViewBag.HasTenants = _settingConfiguration.HasTenants;
		}

		public async Task<ActionResult> Create()
		{
			ViewData["Title"] = "Novo Usuário";
			await LoadViewBags();
			ViewBagMenu();
			return View();
		}

		[HttpPost]
		public async Task<JsonResult> GetExistingEmail(string login)
		{
			try
			{
				var result = await _userService.GetExistingEmail(login);
				return Json(new { ok = result });
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "[GetExistingEmail]: {0}", login);
				return Json(new { ok = false, mensagem = ex.Message });
			}
		}

		[HttpPost]
		public async Task<JsonResult> IsTenantRole(string idRole)
		{
			try
			{
				var result = await _userService.IsTenantRole(idRole);
				return Json(new { ok = true, isTenantRole = result });
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "[IsTenantRole]: {0}", idRole);
				return Json(new { ok = false, mensagem = ex.Message });
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(UserCreateModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					await _userService.Create(new IdentityUserModel()
					{
						Email = model.Email,
						Name = model.Name,
						NormalizedEmail = model.Email.ToUpper(),
						NormalizedUserName = model.Email.ToUpper(),
						UserName = model.Email,
						PhoneNumber = model.Phone,
						Token = model.Password,
						Role = new ApplicationRole() { Id = model.IdRole },
						TenantId = model.IdTenant
					}, _settingConfiguration);
					return RedirectToAction("Index", new { mensagem = true });
				}
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, ex.Message);
				logger.LogError("Create", ex);
			}
			await LoadViewBags();
			ViewBagMenu();
			return View(model);
		}

		public async Task<ActionResult> Edit(string id)
		{
			ViewData["Title"] = "Editar Usuário";
			await LoadViewBags();
			var user = await _userService.Get(id);
			var model = new UserEditModel()
			{
				Id = id,
				Name = user.Name,
				Email = user.Email,
				IdRole = user.Role.Id,
				Phone = user.PhoneNumber,
				IdTenant = user.TenantId
			};

			ViewBagMenu();
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Edit(UserEditModel model)
		{
			try
			{
				await LoadViewBags();
				if (ModelState.IsValid)
				{
					var user = await _userService.Set(new IdentityUserModel()
					{
						Name = model.Name,
						NormalizedEmail = model.Email.ToUpper(),
						NormalizedUserName = model.Email.ToUpper(),
						Email = model.Email,
						UserName = model.Email,
						PhoneNumber = model.Phone,
						SecurityStamp = new Guid().ToString(),
						Role = new ApplicationRole() { Id = model.IdRole },
						TenantId = model.IdTenant
					});
					if (user != null)
						return RedirectToAction("Index", new { mensagem = true });
				}
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, ex.Message);
				logger.LogError("Edit", ex);
			}

			ViewBagMenu();
			return View(model);
		}

		[HttpPost]
		public async Task<ActionResult> Delete(string id)
		{
			try
			{
				await _userService.Delete(id);
				return Json(new { ok = true });
			}
			catch (ApplicationException ex)
			{
				return Json(new { ok = false, mensagem = ex.Message });
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "[Delete]: {0}", id);
				return Json(new { ok = false, mensagem = ex.ToString() });
			}
		}
	}
}
