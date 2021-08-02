using System.ComponentModel.DataAnnotations;

namespace SSO.Infrastructure.Business.Model
{
	public class UserCreateModel
	{
		public string Id { get; set; }

		[Display(Name = "Nome")]
		[Required(ErrorMessage = "Campo obrigatório")]
		[StringLength(255)]
		public string Name { get; set; }

		[Display(Name = "Telefone")]
		[StringLength(255)]
		public string Phone { get; set; }

		[Display(Name = "E-mail")]
		[Required(ErrorMessage = "Campo obrigatório")]
		[RegularExpression("^[\\w\\-]+(\\.[\\w\\-]+)*@([A-Za-z0-9-]+\\.)+[A-Za-z]{2,4}$", ErrorMessage = "Endereço de E-mail inválido")]
		[DataType(DataType.EmailAddress)]
		[StringLength(255)]
		public string Email { get; set; }

		[Display(Name = "Perfil")]
		[Required(ErrorMessage = "Campo obrigatório")]
		public string IdRole { get; set; }

		[Display(Name = "Senha")]
		[StringLength(100, ErrorMessage = "O campo {0} deve ter ao menos {2} caracteres", MinimumLength = 8)]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Display(Name = "Confirmar Senha")]
		[StringLength(100, ErrorMessage = "O campo {0} deve ter ao menos {2} caracteres", MinimumLength = 8)]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "A Senha e a Confirmação de Senha não conferem")]
		public string PasswordConfirmation { get; set; }

		[Display(Name = "Cliente")]
		public int? IdTenant { get; set; }
	}
}
