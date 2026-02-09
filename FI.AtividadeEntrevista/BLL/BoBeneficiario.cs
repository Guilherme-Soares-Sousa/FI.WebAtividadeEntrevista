
using System.Collections.Generic;
using FI.AtividadeEntrevista.DAL.Beneficiario;
using FI.AtividadeEntrevista.DML;

namespace FI.AtividadeEntrevista.BLL
{
    public class BoBeneficiario
    {
        public List<Beneficiario> ListarBeneficiario(long IdCliente)
        {
			DaoBeneficiario dao = new DaoBeneficiario();
            return dao.Listar(IdCliente);
		}

        public void IncluirBeneficiario(Beneficiario beneficiario)
        {
			DaoBeneficiario dao = new DaoBeneficiario();
            dao.IncluirBeneficiario(beneficiario);
		}
		public bool VerificarExistenciaBeneficiario(long Id, string CPF)
		{
			DaoBeneficiario dao = new DaoBeneficiario();
			return dao.VerificarExistenciaBeneficiario(Id, CPF);
		}

		public void AtualizaBeneficiario(Beneficiario beneficiario)
		{
			DaoBeneficiario dao = new DaoBeneficiario();
			dao.AtualizaBeneficiario(beneficiario);
		}

		public void ExcluirBenef(long id)
		{
			DaoBeneficiario benef = new DaoBeneficiario();
			benef.Excluir(id);
		}
	}
}
