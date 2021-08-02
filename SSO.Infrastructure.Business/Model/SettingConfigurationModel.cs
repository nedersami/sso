namespace SSO.Infrastructure.Business.Model
{
	public class SettingConfigurationModel
	{
		public string SecretKey { get; set; }
		public string UrlDefaultApp { get; set; }
		public string ApplicationName { get; set; }
		public bool HasTenants { get; set; }
		public string SMTP_Host { get; set; }
		public string SMTP_Port { get; set; }
		public string SMTP_User { get; set; }
		public string SMTP_Pass { get; set; }
		public string SMTP_From { get; set; }
		public string SMTP_From_Alias { get; set; }
	}
}
