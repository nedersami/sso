using SSO.Infrastructure.Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSO.API.Helpers
{
	public static class ErrorDictionary
	{
		private static Dictionary<string, string> ErrorList = new Dictionary<string, string>
		{
			{"-1", "Exception" },
			{"0", "ModelState" },
			{"AUT-01","Login e/ou Senha inválidos" },
			{"AUT-M01","Login é obrigatório" },
			{"AUT-M02", "Senha é obrigatório" }
		};

		public static ErrorModel GetError(string errorCode)
		{
			var erro = ErrorList.Select(q => new ErrorModel(q.Key, q.Value)).FirstOrDefault(q => q.ErrorCode == errorCode);
			if (erro == null)
				erro = new ErrorModel("-1", "Código de erro não encontrado");
			return erro;
		}

		public static List<ErrorModel> GetErrors()
		{
			return ErrorList.Select(q => new ErrorModel(q.Key, q.Value)).ToList();
		}
	}
}
