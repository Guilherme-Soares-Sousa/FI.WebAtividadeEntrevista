using System.Collections.Generic;
using System.Web.Mvc;
using FI.AtividadeEntrevista.BLL;
using FI.AtividadeEntrevista.DML;
using FI.WebAtividadeEntrevista.Models;

namespace WebAtividadeEntrevista.Controllers {
	public class BeneficiarioController : Controller
	{
		[HttpGet]
		public JsonResult ListarBeneficiario(long Id)
		{
			BoBeneficiario beneficiario = new BoBeneficiario();
			List<BeneficiarioModel> beneficiarios = new List<BeneficiarioModel>();
			beneficiarios = obterLista(beneficiario.ListarBeneficiario(Id));
			return Json(beneficiarios, JsonRequestBehavior.AllowGet);
		}

		private	List<BeneficiarioModel> obterLista(List<Beneficiario> beneficiarios)
		{
			List<BeneficiarioModel> listaBeneficiarioModel = new List<BeneficiarioModel>();
			foreach (var beneficiario in beneficiarios)
			{
				listaBeneficiarioModel.Add(new BeneficiarioModel
				{
					Id = beneficiario.Id,
					CPF = beneficiario.CPF,
					Nome = beneficiario.Nome,
					IdCliente = beneficiario.IdCliente,
				});
			}
			return listaBeneficiarioModel;
		}
	}
}