using System.ComponentModel.DataAnnotations;

namespace SSO.Infrastructure.Business.Model
{
	public class ForgotPasswordModel
	{
		[Display(Name = "E-mail")]
		[Required(ErrorMessage = "Campo obrigatório")]
		[RegularExpression("^[\\w\\-]+(\\.[\\w\\-]+)*@([A-Za-z0-9-]+\\.)+[A-Za-z]{2,4}$", ErrorMessage = "Endereço de E-mail inválido")]
		[DataType(DataType.EmailAddress)]
		[StringLength(255)]
		public string Email { get; set; }
	}
}
