using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using SSO.API.Helpers;
using SSO.Infrastructure.Business.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SSO.API.Controllers
{
	[ApiController]
	public class RoleController : BaseController
	{
		private RoleManager<IdentityRole> _roleManager;
		private readonly ILogger _logger;

		public RoleController(
			RoleManager<IdentityRole> roleManager,
			ILoggerFactory loggerFactory,
			IMapper mapper)
		{
			_logger = loggerFactory.CreateLogger<AccountController>();
			_roleManager = roleManager;
		}

		/// <summary>
		/// Responsável por obter a lista de roles
		/// </summary>
		[HttpGet]
		[Authorize("Bearer", Roles = "Administrador")]
		[ProducesResponseType(typeof(IdentityRole[]), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(BadRequestResponseModel), (int)HttpStatusCode.BadRequest)]
		public IActionResult GetRoles()
		{
			IList<IdentityRole> retorno = null;

			try
			{
				var roles = _roleManager.Roles.ToArray();
				if (roles != null && roles.Length > 0)
				{
					retorno = roles;
				}
				else
					throw new BusinessException("AUT-01");
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
		/// Responsável por inserir uma role
		/// </summary>
		[HttpPost]
		[ProducesResponseType(typeof(IdentityRole), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(BadRequestResponseModel), (int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> CreateRole(IdentityRole role)
		{
			IdentityRole retorno = null;

			try
			{
				var result = await _roleManager.CreateAsync(role);
				if (result.Succeeded)
					retorno = await _roleManager.FindByNameAsync(role.Name);
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
		/// Responsável por obter uma role
		/// </summary>
		/// <param name="id"></param>
		[HttpGet]
		[Route("id/{id}")]
		[ProducesResponseType(typeof(IdentityRole), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(BadRequestResponseModel), (int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> GetRole(string id)
		{
			IdentityRole retorno = null;

			try
			{
				var result = await _roleManager.FindByIdAsync(id);
				if (result != null)
				{
					retorno = result;
				}
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
		/// Responsável por atualizar uma role
		/// </summary>
		/// <param name="role"></param>
		[HttpPut]
		[ProducesResponseType(typeof(IdentityRole), (int)HttpStatusCode.OK)]
		[ProducesResponseType(typeof(BadRequestResponseModel), (int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> UpdateRole(IdentityRole role)
		{
			IdentityRole retorno = null;

			try
			{
				var result = await _roleManager.UpdateAsync(role);
				if (result.Succeeded)
					retorno = await _roleManager.FindByNameAsync(role.Name);
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