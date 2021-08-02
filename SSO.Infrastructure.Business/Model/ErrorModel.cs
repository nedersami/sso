namespace SSO.Infrastructure.Business.Model
{
	public class ErrorModel
	{
		public ErrorModel(string errorCode, string errorMessage)
		{
			ErrorCode = errorCode;
			ErrorMessage = errorMessage;
		}

		public string ErrorCode { get; private set; }
		public string ErrorMessage { get; private set; }
	}
}
