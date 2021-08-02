using System.ComponentModel.DataAnnotations;

namespace SSO.Infrastructure.Business.Model
{
	public class ChangePasswordModel
	{
		[Display(Name = "Senha Atual")]
		[Required(ErrorMessage = "Campo obrigatório")]
		[StringLength(100, ErrorMessage = "O campo {0} deve ter ao menos {2} caracteres", MinimumLength = 8)]
		[DataType(DataType.Password)]
		public string SenhaAtual { get; set; }


		[Display(Name = "Nova Senha")]
		[Required(ErrorMessage = "Campo obrigatório")]
		[StringLength(100, ErrorMessage = "O campo {0} deve ter ao menos {2} caracteres", MinimumLength = 8)]
		[DataType(DataType.Password)]
		public string Senha { get; set; }

		[Display(Name = "Confirmar Senha")]
		[Required(ErrorMessage = "Campo obrigatório")]
		[StringLength(100, ErrorMessage = "O campo {0} deve ter ao menos {2} caracteres", MinimumLength = 8)]
		[DataType(DataType.Password)]
		[Compare("Senha", ErrorMessage = "A Senha e a Confirmação de Senha não conferem")]
		public string ConfirmacaoSenha { get; set; }
	}
}
