using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;
using FI.WebAtividadeEntrevista.Models;

namespace WebAtividadeEntrevista.Controllers
{
	public class ClienteController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}


		public ActionResult Incluir()
		{
			// Limpa beneficiários temporários ao entrar na tela
			LimparBeneficiariosSessao();
			return View();
		}

		[HttpPost]
		public JsonResult Incluir(ClienteModel model)
		{
			BoCliente bo = new BoCliente();
			BoBeneficiario boBenef = new BoBeneficiario();
			if (!this.ModelState.IsValid)
			{
				List<string> erros = (from item in ModelState.Values
									  from error in item.Errors
									  select error.ErrorMessage).ToList();

				Response.StatusCode = 400;
				return Json(string.Join(Environment.NewLine, erros));
			}
			else
			{
				List<BeneficiarioModel> beneficiarios = Session["BeneficiariosTemporal"] as List<BeneficiarioModel>;
				if (!bo.VerificarExistencia(model.Id,model.CPF) && !ValidarCPFBenef(beneficiarios,0))
				{
					model.Id = bo.Incluir(new Cliente()
					{
						CEP = model.CEP,
						Cidade = model.Cidade,
						Email = model.Email,
						Estado = model.Estado,
						Logradouro = model.Logradouro,
						Nacionalidade = model.Nacionalidade,
						Nome = model.Nome,
						Sobrenome = model.Sobrenome,
						CPF = model.CPF,
						Telefone = model.Telefone
					});


					if (!SalvarBeneficiariosBanco(model.Id))
					{
						Response.StatusCode = 400;
						return Json("CPF existente em base!");
					};

					return Json("Cadastro efetuado com sucesso");
				}

				Response.StatusCode = 400;
				return Json("CPF existente em base!");
			}
		}

		[HttpPost]
		public JsonResult Alterar(ClienteModel model)
		{
			BoCliente bo = new BoCliente();

			if (!this.ModelState.IsValid)
			{
				List<string> erros = (from item in ModelState.Values
									  from error in item.Errors
									  select error.ErrorMessage).ToList();

				Response.StatusCode = 400;
				return Json(string.Join(Environment.NewLine, erros));
			}
			else
			{
				List<BeneficiarioModel> beneficiarios = Session["BeneficiariosTemporal"] as List<BeneficiarioModel>;
				if (!bo.VerificarExistencia(model.Id, model.CPF) && !ValidarCPFBenef(beneficiarios, model.Id))
				{
					if (!bo.VerificarExistencia(model.Id, model.CPF))
					{
						bo.Alterar(new Cliente()
						{
							Id = model.Id,
							CEP = model.CEP,
							Cidade = model.Cidade,
							Email = model.Email,
							Estado = model.Estado,
							Logradouro = model.Logradouro,
							Nacionalidade = model.Nacionalidade,
							Nome = model.Nome,
							Sobrenome = model.Sobrenome,
							CPF = model.CPF,
							Telefone = model.Telefone
						});

						SalvarAlteracoesBeneficiariosBanco(model.Id);

						return Json("Alteração efetuada com sucesso");
					}
				}
					

				Response.StatusCode = 400;
				return Json("CPF existente em base!");
			}
		}

		[HttpGet]
		public ActionResult Alterar(long id)
		{
			// Limpa beneficiários temporários ao entrar na tela
			LimparBeneficiariosSessao();

			BoCliente bo = new BoCliente();
			Cliente cliente = bo.Consultar(id);
			Models.ClienteModel model = null;

			if (cliente != null)
			{
				model = new ClienteModel()
				{
					Id = cliente.Id,
					CEP = cliente.CEP,
					Cidade = cliente.Cidade,
					Email = cliente.Email,
					Estado = cliente.Estado,
					Logradouro = cliente.Logradouro,
					Nacionalidade = cliente.Nacionalidade,
					Nome = cliente.Nome,
					Sobrenome = cliente.Sobrenome,
					CPF = cliente.CPF,
					Telefone = cliente.Telefone
				};
			}

			return View(model);
		}

		[HttpPost]
		public JsonResult Excluir(long id)
		{
			BoCliente bo = new BoCliente();
			bo.Excluir(id);

			return Json("Usuário excluído com sucesso", JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult ObterBeneficiariosSessao()
		{
			List<BeneficiarioModel> beneficiarios = Session["BeneficiariosTemporal"] as List<BeneficiarioModel>;
			if (beneficiarios == null)
			{
				beneficiarios = new List<BeneficiarioModel>();
			}

			List<string> excluidos = Session["BeneficiariosExcluidos"] as List<string>;
			if (excluidos == null)
			{
				excluidos = new List<string>();
			}

			return Json(new { beneficiarios = beneficiarios, excluidos = excluidos }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult VerificarCPFExistente(string cpf)
		{
			if (string.IsNullOrWhiteSpace(cpf))
				return Json(new { Exists = false, Client = false, Beneficiario = false }, JsonRequestBehavior.AllowGet);

			BoCliente bo = new BoCliente();
			BoBeneficiario boBenef= new BoBeneficiario();
			bool existsClient = bo.VerificarExistencia(0, cpf);
			bool existsBenef = boBenef.VerificarExistenciaBeneficiario(0, cpf);

			return Json(new { Exists = (existsClient || existsBenef), Client = existsClient, Beneficiario = existsBenef }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult SalvarBeneficiarioSession(BeneficiarioModel beneficiario, long? IdCliente = null)
		{
		if (!this.ModelState.IsValid)
		{
		List<string> erros = (from item in ModelState.Values
				from error in item.Errors
				select error.ErrorMessage).ToList();

		Response.StatusCode = 400;
		return Json(string.Join(Environment.NewLine, erros));
		}

		string cpfLimpo = beneficiario.CPF.Replace(".", "").Replace("-", "");

		// Inicializa lista de beneficiários em session se não existir
		List<BeneficiarioModel> beneficiarios = Session["BeneficiariosTemporal"] as List<BeneficiarioModel>;
		if (beneficiarios == null)
		{
		beneficiarios = new List<BeneficiarioModel>();
		}

		// Verifica se já existe beneficiário com mesmo CPF na sessão
		if (beneficiarios.Any(b => b.CPF.Replace(".", "").Replace("-", "") == cpfLimpo))
		{
		Response.StatusCode = 400;
		return Json("CPF já existe na lista de beneficiários");
		}

		// Se estiver em modo de alteração (IdCliente informado), verifica também os beneficiários do banco
		if (IdCliente.HasValue && IdCliente.Value > 0)
		{
		BoBeneficiario boBenef = new BoBeneficiario();
		var beneficiariosBanco = boBenef.ListarBeneficiario(IdCliente.Value);

		// Considera a lista de excluídos — CPFs removidos do grid podem ser reutilizados
		List<string> excluidos = Session["BeneficiariosExcluidos"] as List<string>;
		if (excluidos == null) excluidos = new List<string>();

		if (beneficiariosBanco != null && beneficiariosBanco.Any(b =>
			b.CPF.Replace(".", "").Replace("-", "") == cpfLimpo &&
			!excluidos.Contains(b.CPF.Replace(".", "").Replace("-", ""))))
		{
		Response.StatusCode = 400;
		return Json("CPF já existe na lista de beneficiários");
		}
		}

		// Adiciona novo beneficiário
		beneficiarios.Add(new BeneficiarioModel
		{
		CPF = beneficiario.CPF,
		Nome = beneficiario.Nome
			});

			// Salva em session
			Session["BeneficiariosTemporal"] = beneficiarios;

			return Json(new { success = true, message = "Beneficiário adicionado com sucesso" });
		}

		[HttpPost]
		public JsonResult AtualizarBeneficiarioSession(string originalCpf, string CPF, string Nome, long? IdCliente = null)
		{
			// Mantém alterações SOMENTE em Session. Não persiste no banco aqui.
			List<BeneficiarioModel> beneficiarios = Session["BeneficiariosTemporal"] as List<BeneficiarioModel>;
			if (beneficiarios == null)
				beneficiarios = new List<BeneficiarioModel>();

			string originalCpfLimpo = (originalCpf ?? string.Empty).Replace(".", "").Replace("-", "");
			string cpfLimpo = (CPF ?? string.Empty).Replace(".", "").Replace("-", "");

			// Se o CPF mudou, verifica duplicidade
			if (cpfLimpo != originalCpfLimpo)
			{
				// Evita duplicidade de CPF dentro da própria Session (considerando formatação)
				if (beneficiarios.Any(b =>
					b.CPF != null &&
					b.CPF.Replace(".", "").Replace("-", "") == cpfLimpo &&
					b.CPF.Replace(".", "").Replace("-", "") != originalCpfLimpo))
				{
					Response.StatusCode = 400;
					return Json("CPF já existe na lista de beneficiários");
				}

				// Evita duplicidade contra beneficiários do banco (exceto o próprio)
				if (IdCliente.HasValue && IdCliente.Value > 0)
				{
					BoBeneficiario boBenefCheck = new BoBeneficiario();
					var beneficiariosBancoCheck = boBenefCheck.ListarBeneficiario(IdCliente.Value);
					if (beneficiariosBancoCheck != null && beneficiariosBancoCheck.Any(b =>
						(b.CPF ?? string.Empty).Replace(".", "").Replace("-", "") == cpfLimpo &&
						(b.CPF ?? string.Empty).Replace(".", "").Replace("-", "") != originalCpfLimpo))
					{
						Response.StatusCode = 400;
						return Json("CPF já existe na lista de beneficiários");
					}
				}
			}

			// Primeiro tenta atualizar na Session (buscando por CPF atual ou CPF original)
			var item = beneficiarios.FirstOrDefault(b =>
				(b.CPF ?? string.Empty).Replace(".", "").Replace("-", "") == originalCpfLimpo ||
				(b.OriginalCPF ?? string.Empty).Replace(".", "").Replace("-", "") == originalCpfLimpo);
			if (item != null)
			{
				item.CPF = CPF;
				item.Nome = Nome;
				item.OriginalCPF = item.OriginalCPF ?? originalCpf;
				Session["BeneficiariosTemporal"] = beneficiarios;
				return Json(new { success = true, message = "Beneficiário atualizado com sucesso" });
			}

			// Se ainda não existe na Session, mas existe no banco, adiciona apenas o beneficiário alterado na Session.
			if (IdCliente.HasValue && IdCliente.Value > 0)
			{
				BoBeneficiario boBenef = new BoBeneficiario();
				var beneficiariosBanco = boBenef.ListarBeneficiario(IdCliente.Value);
				var benefBanco = beneficiariosBanco?.FirstOrDefault(b => (b.CPF ?? string.Empty).Replace(".", "").Replace("-", "") == originalCpfLimpo);

				if (benefBanco != null)
				{
					beneficiarios.Add(new BeneficiarioModel
					{
						CPF = CPF,
						Nome = Nome,
						OriginalCPF = originalCpf
					});

					Session["BeneficiariosTemporal"] = beneficiarios;
					return Json(new { success = true, message = "Beneficiário atualizado com sucesso" });
				}
			}

			return Json(new { success = false, message = "Beneficiário não encontrado" });
		}

		[HttpPost]
		public JsonResult ExcluirBeneficiarioSession(string CPF)
		{
			string cpfLimpo = (CPF ?? string.Empty).Replace(".", "").Replace("-", "");

			List<BeneficiarioModel> beneficiarios = Session["BeneficiariosTemporal"] as List<BeneficiarioModel>;
			if (beneficiarios == null)
				beneficiarios = new List<BeneficiarioModel>();

			var item = beneficiarios.FirstOrDefault(b =>
				(b.CPF ?? string.Empty).Replace(".", "").Replace("-", "") == cpfLimpo);

			if (item != null)
			{
				// Se o item possuía OriginalCPF, registra o CPF original na lista de excluídos
				if (!string.IsNullOrEmpty(item.OriginalCPF))
				{
					AdicionarCpfExcluido(item.OriginalCPF);
				}
				beneficiarios.Remove(item);
				Session["BeneficiariosTemporal"] = beneficiarios;
				return Json(new { success = true, message = "Beneficiário removido com sucesso" });
			}

			// Não estava na sessão — é um registro que veio do banco.
			// Registra o CPF na lista de excluídos para que não apareça no grid.
			AdicionarCpfExcluido(CPF);
			return Json(new { success = true, message = "Beneficiário removido com sucesso" });
		}

		private void AdicionarCpfExcluido(string cpf)
		{
			string cpfLimpo = (cpf ?? string.Empty).Replace(".", "").Replace("-", "");
			List<string> excluidos = Session["BeneficiariosExcluidos"] as List<string>;
			if (excluidos == null)
				excluidos = new List<string>();

			if (!excluidos.Contains(cpfLimpo))
				excluidos.Add(cpfLimpo);

			Session["BeneficiariosExcluidos"] = excluidos;
		}

		private bool ValidarCPFBenef(List<BeneficiarioModel> listaBenefModel, long IdCliente)
		{
			BoBeneficiario boBenef = new BoBeneficiario();
			if (listaBenefModel != null && listaBenefModel.Count > 0)
			{
				foreach (var benefLista in listaBenefModel)
				{
					// Se o beneficiário manteve seu próprio CPF (não alterou), não precisa validar contra o banco
					if (!string.IsNullOrEmpty(benefLista.OriginalCPF))
					{
						string cpfAtualLimpo = (benefLista.CPF ?? string.Empty).Replace(".", "").Replace("-", "");
						string cpfOriginalLimpo = benefLista.OriginalCPF.Replace(".", "").Replace("-", "");
						if (cpfAtualLimpo == cpfOriginalLimpo)
							continue;
					}

					if (boBenef.VerificarExistenciaBeneficiario(IdCliente, benefLista.CPF))
						return true;
				}
			}
			return false;
		}

		private void SalvarAlteracoesBeneficiariosBanco(long idCliente)
		{
			BoBeneficiario bo = new BoBeneficiario();
			var beneficiariosBanco = bo.ListarBeneficiario(idCliente);

			// 1) Exclui beneficiários que foram removidos do grid
			List<string> excluidos = Session["BeneficiariosExcluidos"] as List<string>;
			if (excluidos != null && excluidos.Count > 0 && beneficiariosBanco != null)
			{
				foreach (var cpfExcluido in excluidos)
				{
					var benefBanco = beneficiariosBanco.FirstOrDefault(b =>
						(b.CPF ?? string.Empty).Replace(".", "").Replace("-", "") == cpfExcluido);
					if (benefBanco != null)
					{
						bo.ExcluirBenef(benefBanco.Id);
					}
				}
			}

			// 2) Processa beneficiários da sessão (alterados e novos)
			List<BeneficiarioModel> beneficiarios = Session["BeneficiariosTemporal"] as List<BeneficiarioModel>;
			if (beneficiarios != null && beneficiarios.Count > 0 && beneficiariosBanco != null)
			{
				foreach (var benefSessao in beneficiarios)
				{
					if (!string.IsNullOrEmpty(benefSessao.OriginalCPF))
					{
						// Beneficiário existente no banco que foi alterado
						string originalLimpo = benefSessao.OriginalCPF.Replace(".", "").Replace("-", "");
						var benefBanco = beneficiariosBanco.FirstOrDefault(b =>
							(b.CPF ?? string.Empty).Replace(".", "").Replace("-", "") == originalLimpo);
						if (benefBanco != null)
						{
							bo.AtualizaBeneficiario(new Beneficiario()
							{
								Id = benefBanco.Id,
								CPF = benefSessao.CPF,
								Nome = benefSessao.Nome,
								IdCliente = idCliente
							});
						}
					}
					else
					{
						// Beneficiário novo adicionado na sessão
						bo.IncluirBeneficiario(new Beneficiario()
						{
							CPF = benefSessao.CPF,
							Nome = benefSessao.Nome,
							IdCliente = idCliente
						});
					}
				}
			}

			LimparBeneficiariosSessao();
		}

		private bool SalvarBeneficiariosBanco(long idCliente)
		{
			// Recupera beneficiários da session
			List<BeneficiarioModel> beneficiarios = Session["BeneficiariosTemporal"] as List<BeneficiarioModel>;
			
			if (beneficiarios != null && beneficiarios.Count > 0)
			{
				BoBeneficiario bo = new BoBeneficiario();
			foreach (var beneficiario in beneficiarios)
			{
				// Verifica se o CPF do beneficiário é igual ao do cliente
				BoCliente boCliente = new BoCliente();
				Cliente clienteAtual = boCliente.Consultar(idCliente);
				
				if (clienteAtual != null && beneficiario.CPF.Replace(".", "").Replace("-", "") == clienteAtual.CPF.Replace(".", "").Replace("-", ""))
				{
					return false;
				}

				// Se passou nas validações, insere
				bo.IncluirBeneficiario(new Beneficiario() { CPF = beneficiario.CPF, Nome = beneficiario.Nome, IdCliente = idCliente });
			}
				
			// Limpa a session após salvar
			LimparBeneficiariosSessao();
			}
			return true;
		}

		private void LimparBeneficiariosSessao()
		{
			// Remove a session de beneficiários temporários
			if (Session["BeneficiariosTemporal"] != null)
			{
				Session.Remove("BeneficiariosTemporal");
			}
			if (Session["BeneficiariosExcluidos"] != null)
			{
				Session.Remove("BeneficiariosExcluidos");
			}
		}

		[HttpPost]
		public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
		{
			try
			{
				int qtd = 0;
				string campo = string.Empty;
				string crescente = string.Empty;
				string[] array = jtSorting.Split(' ');

				if (array.Length > 0)
					campo = array[0];

				if (array.Length > 1)
					crescente = array[1];

				List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

				//Return result to jTable
				return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
			}
			catch (Exception ex)
			{
				return Json(new { Result = "ERROR", Message = ex.Message });
			}
		}
	}
}