using SSO.Infrastructure.Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSO.API.Helpers
{
	public class BusinessException : Exception
	{
		public BusinessException(string errorCode)
		{
			ErrorCode = errorCode;
		}

		public string ErrorCode { get; private set; }

		public ErrorModel Erro
		{
			get
			{
				return ErrorDictionary.GetError(ErrorCode);
			}
		}
	}
}
