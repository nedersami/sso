using System.ComponentModel.DataAnnotations;

namespace SSO.Infrastructure.Business.Model
{
	public class ChangeResetPasswordModel
	{
		[Display(Name = "E-mail")]
		[Required(ErrorMessage = "Campo obrigatório")]
		[RegularExpression("^[\\w\\-]+(\\.[\\w\\-]+)*@([A-Za-z0-9-]+\\.)+[A-Za-z]{2,4}$", ErrorMessage = "Endereço de E-mail inválido")]
		[DataType(DataType.EmailAddress)]
		[StringLength(255)]
		public string Email { get; set; }

		[Display(Name = "Nova Senha")]
		[StringLength(100, ErrorMessage = "O campo {0} deve ter ao menos {2} caracteres", MinimumLength = 8)]
		[DataType(DataType.Password)]
		public string Senha { get; set; }

		[Display(Name = "Confirmar Senha")]
		[StringLength(100, ErrorMessage = "O campo {0} deve ter ao menos {2} caracteres", MinimumLength = 8)]
		[DataType(DataType.Password)]
		[Compare("Senha", ErrorMessage = "A Senha e a Confirmação de Senha não conferem")]
		public string ConfirmacaoSenha { get; set; }

		[Required]
		public string Token { get; set; }
	}
}
