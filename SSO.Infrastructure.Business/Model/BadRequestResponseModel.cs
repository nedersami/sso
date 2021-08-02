using System.Collections.Generic;
using System.Linq;

namespace SSO.Infrastructure.Business.Model
{
	public class BadRequestResponseModel
	{
		public BadRequestResponseModel(List<ErrorModel> erros)
		{
			Errors = erros;
		}

		public bool BusinessError => Errors != null && Errors.Any(q => q.ErrorCode != "-1");

		public List<ErrorModel> Errors { get; private set; }
	}
}
