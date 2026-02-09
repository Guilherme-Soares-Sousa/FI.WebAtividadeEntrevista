using System.ComponentModel.DataAnnotations;
using WebAtividadeEntrevista.Validation;

namespace FI.WebAtividadeEntrevista.Models
{
    public class BeneficiarioModel
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
		[ValidationCPF]
		public string CPF  { get; set; }

        [Required]
        public long IdCliente { get; set; }

		public string OriginalCPF { get; set; }
    }
}
