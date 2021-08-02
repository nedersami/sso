using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SSO.API.Helpers;
using SSO.Infrastructure.Business.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace SSO.API.Controllers
{
	[Produces("application/json")]
	[Route("[controller]")]
	public class BaseController : ControllerBase
	{
		protected List<ErrorModel> Errors { get; set; }

		protected new IActionResult Response(object result = null)
		{
			if (OK())
				return Ok(result);

			return BadRequest(new BadRequestResponseModel(Errors));
		}

		protected bool OK()
		{
			return Errors == null || !Errors.Any();
		}

		protected void NotifyError(Exception ex)
		{
			NotificarErro("-1", $"{ex.Message}-{ex.StackTrace}");
		}

		protected void NotifyError(BusinessException ex)
		{
			NotifyError(ex.Erro);
		}

		protected void NotifyError(ErrorModel erro)
		{
			if (Errors == null) Errors = new List<ErrorModel>();
			Errors.Add(erro);
		}

		protected void NotifyError(string codigoErro)
		{
			NotifyError(ErrorDictionary.GetError(codigoErro));
		}

		protected void AddIdentityErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				NotificarErro("-1", error.Description);
			}
		}

		private void NotificarErro(string errorCode, string errorMessage)
		{
			if (Errors == null) Errors = new List<ErrorModel>();
			Errors.Add(new ErrorModel(errorCode, errorMessage));
		}
	}
}