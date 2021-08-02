using System.ComponentModel.DataAnnotations;

namespace SSO.Infrastructure.Business.Model
{
	public class LoginModel
	{
		[Required]
		[Display(Name = "Usuário")]
		public string UserName { get; set; }

		[Required]
		[Display(Name = "Senha")]
		public string Password { get; set; }


		[Display(Name = "Lembrar Usuário")]
		public bool Rememberme { get; set; }
	}
}
