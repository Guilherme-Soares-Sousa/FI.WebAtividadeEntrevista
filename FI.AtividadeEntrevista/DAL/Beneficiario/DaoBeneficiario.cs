using System.Collections.Generic;
using System.Data;
using FI.AtividadeEntrevista.DML;

namespace FI.AtividadeEntrevista.DAL.Beneficiario
{
	internal class DaoBeneficiario : AcessoDados
	{
		/// <summary>
		/// Lista todos os beneficiarios
		/// </summary>
		internal List<DML.Beneficiario> Listar(long IdCliente)
		{
			List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

			parametros.Add(new System.Data.SqlClient.SqlParameter("IdCliente",IdCliente));

			DataSet ds = base.Consultar("FI_SP_ListaBeneficiarioV1", parametros);
			List<DML.Beneficiario> beneficiario = Converter(ds);

			return beneficiario;
		}

		internal void IncluirBeneficiario(DML.Beneficiario beneficiario)
		{
			List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

			parametros.Add(new System.Data.SqlClient.SqlParameter("CPF", beneficiario.CPF));
			parametros.Add(new System.Data.SqlClient.SqlParameter("NOME", beneficiario.Nome));
			parametros.Add(new System.Data.SqlClient.SqlParameter("IDCLIENTE", beneficiario.IdCliente));
			base.Consultar("FI_SP_IncluirBeneficiarioV1", parametros);
		}

		internal bool VerificarExistenciaBeneficiario(long Id, string CPF)
		{
			List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

			parametros.Add(new System.Data.SqlClient.SqlParameter("Id", Id));

			parametros.Add(new System.Data.SqlClient.SqlParameter("CPF", CPF));

			DataSet ds = base.Consultar("FI_SP_VerificaExistenciaBeneficiarioV1", parametros);

			// Stored procedure returns 1 when duplicate found, 0 otherwise.
			if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
			{
				var val = ds.Tables[0].Rows[0][0];
				int intVal;
				if (val != null && int.TryParse(val.ToString(), out intVal))
				{
					return intVal == 1;
				}
			}
			return false;
		}

		internal void AtualizaBeneficiario(DML.Beneficiario beneficiario)
		{
			List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

			parametros.Add(new System.Data.SqlClient.SqlParameter("ID", beneficiario.Id));
			parametros.Add(new System.Data.SqlClient.SqlParameter("CPF", beneficiario.CPF));
			parametros.Add(new System.Data.SqlClient.SqlParameter("NOME", beneficiario.Nome));
			base.Consultar("FI_SP_AltBenef", parametros);
		}


		private List<DML.Beneficiario> Converter(DataSet ds)
		{
			List<DML.Beneficiario> lista = new List<DML.Beneficiario>();
			if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
			{
				foreach (DataRow row in ds.Tables[0].Rows)
				{
					DML.Beneficiario beneficiario = new DML.Beneficiario();
					beneficiario.Id = row.Field<long>("ID");
					beneficiario.Nome = row.Field<string>("Nome");
					beneficiario.CPF = row.Field<string>("CPF");
					beneficiario.IdCliente = row.Field<long>("IDCLIENTE");
					lista.Add(beneficiario);
				}
			}

			return lista;
		}
		internal void Excluir(long Id)
		{
			List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

			parametros.Add(new System.Data.SqlClient.SqlParameter("Id", Id));

			base.Executar("FI_SP_DelBeneficiario", parametros);
		}
	}
}
