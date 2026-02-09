using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WebAtividadeEntrevista.Validation
{
	/// <summary>
	/// Atributo de validação para CPF brasileiro.
	/// Valida o formato e os dígitos verificadores do CPF.
	/// </summary>
	/// <example>
	/// <code>
	/// public class ClienteViewModel
	/// {
	///     [CpfValidation(ErrorMessage = "CPF inválido")]
	///     public string CPF { get; set; }
	/// }
	/// </code>
	/// </example>
	public class ValidationCPF : ValidationAttribute
	{
		/// <summary>
		/// Construtor padrão com mensagem de erro default
		/// </summary>
		public ValidationCPF()
		{
			ErrorMessage = "O CPF informado é inválido.";
		}

		/// <summary>
		/// Valida se o CPF é válido
		/// </summary>
		/// <param name="value">Valor do campo a ser validado</param>
		/// <param name="validationContext">Contexto da validação</param>
		/// <returns>Resultado da validação</returns>
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
			{
				// Se quiser que seja obrigatório, use também [Required]
				// Aqui consideramos que vazio é válido (campo opcional)
				return ValidationResult.Success;
			}

			string cpf = value.ToString();

			if (ValidarCPF(cpf))
			{
				return ValidationResult.Success;
			}

			return new ValidationResult(ErrorMessage);
		}

		/// <summary>
		/// Valida o CPF usando o algoritmo de verificação dos dígitos
		/// </summary>
		/// <param name="cpf">CPF a ser validado (pode conter máscara)</param>
		/// <returns>True se válido, False se inválido</returns>
		private bool ValidarCPF(string cpf)
		{
			// Remove caracteres não numéricos (máscara)
			cpf = new string(cpf.Where(char.IsDigit).ToArray());

			// CPF deve ter 11 dígitos
			if (cpf.Length != 11)
				return false;

			// Verifica se todos os dígitos são iguais (ex: 111.111.111-11)
			if (cpf.Distinct().Count() == 1)
				return false;

			// Calcula o primeiro dígito verificador
			int soma = 0;
			for (int i = 0; i < 9; i++)
			{
				soma += int.Parse(cpf[i].ToString()) * (10 - i);
			}

			int resto = soma % 11;
			int digitoVerificador1 = resto < 2 ? 0 : 11 - resto;

			// Verifica o primeiro dígito
			if (int.Parse(cpf[9].ToString()) != digitoVerificador1)
				return false;

			// Calcula o segundo dígito verificador
			soma = 0;
			for (int i = 0; i < 10; i++)
			{
				soma += int.Parse(cpf[i].ToString()) * (11 - i);
			}

			resto = soma % 11;
			int digitoVerificador2 = resto < 2 ? 0 : 11 - resto;

			// Verifica o segundo dígito
			return int.Parse(cpf[10].ToString()) == digitoVerificador2;
		}
	}
}