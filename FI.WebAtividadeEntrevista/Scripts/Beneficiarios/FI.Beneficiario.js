$(document).ready(function () {
    // Função carregarBeneficiario será chamada quando o modal for aberto
});

var modalAtualId = null; // Armazena o ID do modal atual

function carregarBeneficiario() {
    // Verifica se está em modo de alteração (cliente com ID) ou inclusão (cliente sem ID)
    // A variável 'obj' vem da view e contém os dados do cliente
    
    // Função auxiliar para gerar HTML de linha com classes corretas
    function gerarLinhaHtml(cpf, nome) {
        return '<tr data-cpf="' + cpf + '">' +
            '<td>' + cpf + '</td>' +
            '<td>' + nome + '</td>' +
            '<td>' +
            '<button type="button" class="btn btn-sm btn-primary btn-alterar">Alterar</button> ' +
            '<button type="button" class="btn btn-sm btn-danger btn-excluir">Excluir</button>' +
            '</td>' +
            '</tr>';
    }
    
    // Se estiver em modo de inclusão (obj não existe ou obj.Id é nulo/undefined)
    if (typeof obj === 'undefined' || !obj || !obj.Id) {
        // Em modo de inclusão, carrega beneficiários da SESSION
        $.ajax({
            url: '/Cliente/ObterBeneficiariosSessao',
            method: 'GET',
            success: function (dados) {
                var lista = dados && dados.beneficiarios ? dados.beneficiarios : [];
                if (lista.length > 0) {
                    var html = '';
                    lista.forEach(function (item) {
                        html += gerarLinhaHtml(item.CPF, item.Nome);
                    });
                    $('#' + modalAtualId + ' #gridBeneficiarios').html(html);
                }
            }
        });
        return;
    }
    
    // Se estiver em modo de alteração (cliente tem ID)
    // Carrega beneficiários do banco E da sessão (novos adicionados)
    $.ajax({
        url: '/Beneficiario/ListarBeneficiario',
        method: 'GET',
        data: {
            Id: obj.Id
        },
        success: function (dadosBanco) {
            // Depois carrega os da sessão (novos adicionados durante esta edição)
            $.ajax({
                url: '/Cliente/ObterBeneficiariosSessao',
                method: 'GET',
                success: function (dados) {
                    var dadosSessao = dados && dados.beneficiarios ? dados.beneficiarios : [];
                    var excluidos = dados && dados.excluidos ? dados.excluidos : [];

                    var html = '';
                    var mapFiltro = {};

                    // Mapeia CPFs excluídos para filtrar registros do banco
                    excluidos.forEach(function (cpf) {
                        mapFiltro[cpf] = true;
                    });

                    // Mapeia os CPFs da sessão (tanto OriginalCPF quanto CPF atual) para identificar quais registros do banco já estão representados na sessão
                    dadosSessao.forEach(function (item) {
                        if (item.OriginalCPF) {
                            var cpfOrigLimpo = item.OriginalCPF.replace(/\D/g, '');
                            mapFiltro[cpfOrigLimpo] = true;
                        }
                        var cpfAtualLimpo = item.CPF.replace(/\D/g, '');
                        mapFiltro[cpfAtualLimpo] = true;
                    });

                    // Primeiro adiciona os do banco, exceto os filtrados (sessão ou excluídos)
                    if (dadosBanco && dadosBanco.length > 0) {
                        dadosBanco.forEach(function (item) {
                            var cpfLimpo = item.CPF.replace(/\D/g, '');
                            if (!mapFiltro[cpfLimpo]) {
                                html += gerarLinhaHtml(item.CPF, item.Nome);
                            }
                        });
                    }
                    
                    // Depois adiciona os da sessão
                    dadosSessao.forEach(function (item) {
                        html += gerarLinhaHtml(item.CPF, item.Nome);
                    });
                    
                    $('#' + modalAtualId + ' #gridBeneficiarios').html(html);
                },
                error: function () {
                    // Fallback para exibir apenas dados do banco em caso de erro na sessão
                    var html = '';
                    if (dadosBanco && dadosBanco.length > 0) {
                        dadosBanco.forEach(function (item) {
                            html += gerarLinhaHtml(item.CPF, item.Nome);
                        });
                    }
                    $('#' + modalAtualId + ' #gridBeneficiarios').html(html);
                }
            });
        },
        error: function (e) {
            console.log('Erro ao carregar beneficiários:', e);
        }
    });
}
function ModalBeneficiario() {
    var random = Math.random().toString().replace('.', '');
    modalAtualId = random; // Armazena o ID do modal atual
    var texto = '<div id="' + random + '" class="modal fade">                                                               ' +
        '        <div class="modal-dialog">                                                                                 ' +
        '            <div class="modal-content">                                                                            ' +
        '                <div class="modal-header">                                                                         ' +
        '                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>         ' +
        '                    <h4 class="modal-title">Beneficiários</h4>                                                     ' +
        '                </div>                                                                                             ' +
        '                <div class="modal-body">                                                                           ' +
        '                    <div class="row align-items-end">                                                                              ' +
        '                        <div class="col-md-4">                                                                      ' +
        '                            <div class="form-group">                                                                ' +
        '                                <label for="CPF">CPF:</label>                                                       ' +
        '                                <input required="required" type="text" class="form-control" id="CPF" name="CPF" placeholder="Ex.: (999.999.999-99)" maxlength="14">' +
        '                            </div>                                                                                  ' +
        '                        </div>                                                                                      ' +
        '                        <div class="col-md-6">                                                                      ' +
        '                            <div class="form-group">                                                                ' +
        '                                <label for="Nome">Nome:</label>                                                     ' +
        '                                <input required="required" type="text" class="form-control" id="Nome" name="Nome" placeholder="Ex.: João" maxlength="50">' +
        '                            </div>                                                                                  ' +
        '                        </div>                                                                                      ' +
        '                        <div class="col-md-2">                                                                      ' +
        '                            <button type="button" class="btn btn-sm btn-success" style="margin-top: 26px;">Incluir</button>                  ' +
        '                        </div>                                                                                      ' +
        '                    </div>                                                                                          ' +
        '                    <div class="row" style="margin-top: 20px;">                                                     ' +
        '                        <div class="col-md-12">                                                                     ' +
        '                            <table class="table table-striped table-bordered">                                      ' +
        '                                <thead>                                                                             ' +
        '                                    <tr>                                                                            ' +
        '                                        <th>CPF</th>                                                                ' +
        '                                        <th>Nome</th>                                                               ' +
        '                                        <th>Ações</th>                                                              ' +
        '                                    </tr>                                                                           ' +
        '                                </thead>                                                                            ' +
        '                                <tbody id="gridBeneficiarios">                                                      ' +
        '                                </tbody>                                                                            ' +
        '                            </table>                                                                                ' +
        '                        </div>                                                                                      ' +
        '                    </div>                                                                                          ' +
        '                </div>                                                                                             ' +
        '                                                                                                                   ' +
        '            </div><!-- /.modal-content -->                                                                         ' +
        '  </div><!-- /.modal-dialog -->                                                                                    ' +
        '</div> <!-- /.modal -->                                                                                        ';

    $('body').append(texto);
    $('#' + random).modal('show');
    
    // Aplica máscara de CPF em tempo real e verifica existência em tempo real
    var cpfInput = $('#' + modalAtualId + ' #CPF');
    var includeBtn = $('#' + modalAtualId + ' .btn-success');

    // feedback element
    var fbId = modalAtualId + '-cpf-feedback';
    if ($('#' + fbId).length === 0) {
        cpfInput.after('<small id="' + fbId + '" class="text-danger" style="display:none; margin-left:6px;"></small>');
    }

    // debounce helper
    function debounce(fn, delay) {
        var t;
        return function () {
            var args = arguments;
            clearTimeout(t);
            t = setTimeout(function () { fn.apply(null, args); }, delay);
        };
    }

    cpfInput.on('input', function () {
        var $this = $(this);
        var v = $this.val().replace(/\D/g, '').slice(0, 11);
        v = v.replace(/(\d{3})(\d)/, '$1.$2');
        v = v.replace(/(\d{3})(\d)/, '$1.$2');
        v = v.replace(/(\d{3})(\d{1,2})$/, '$1-$2');
        $this.val(v);

        // só checa quando houver 11 dígitos
        checkCpfDebounced();
    });

    var checkCpfDebounced = debounce(function () {
        var digits = cpfInput.val().replace(/\D/g, '');
        var feedback = $('#' + fbId);
        if (digits.length !== 11) {
            feedback.hide();
            includeBtn.prop('disabled', false);
            return;
        }

        // Em modo de edição, se o CPF não mudou, não precisa validar duplicidade (é o próprio beneficiário)
        if (modoEdicao) {
            var modoEdicaoDigits = modoEdicao.replace(/\D/g, '');
            if (digits === modoEdicaoDigits) {
                feedback.hide();
                includeBtn.prop('disabled', false);
                return;
            }
        }

        $.ajax({
            url: '/Cliente/VerificarCPFExistente',
            method: 'GET',
            data: { cpf: cpfInput.val() },
            success: function (res) {
                if (res && res.Exists) {
                    var msg = '';
                    if (res.Client) msg = 'CPF já cadastrado como cliente.';
                    else if (res.Beneficiario) msg = 'CPF já cadastrado como beneficiário.';
                    else msg = 'CPF já existente.';
                    feedback.text(msg).show();
                    includeBtn.prop('disabled', true);
                } else {
                    feedback.hide();
                    includeBtn.prop('disabled', false);
                }
            },
            error: function () {
                feedback.hide();
                includeBtn.prop('disabled', false);
            }
        });
    }, 500);

    // Carrega os beneficiários após o modal ser criado
    carregarBeneficiario();

    // modoEdicao armazena o CPF original quando estamos alterando
    var modoEdicao = null;

    // Adiciona evento ao botão Incluir (ou Alterar quando em modo edição)
    $(document).on('click', '#' + random + ' .btn-success', function() {
        // bloqueia se validação em tempo real encontrou duplicidade
        if ($(this).prop('disabled')) {
            alert('CPF inválido ou já existente. Corrija antes de incluir.');
            return;
        }

        var cpf = $('#' + modalAtualId + ' #CPF').val();
        var nome = $('#' + modalAtualId + ' #Nome').val();
        
        if (!cpf || !nome) {
            alert('CPF e Nome são obrigatórios');
            return;
        }

        // Verifica se CPF já existe no grid (sessão atual) - apenas para inclusão
        if (!modoEdicao) {
            var cpfLimpo = cpf.replace(/\D/g, '');
            var cpfDuplicado = false;
            $('#' + modalAtualId + ' #gridBeneficiarios tr').each(function () {
                var cpfLinha = $(this).find('td').eq(0).text().replace(/\D/g, '');
                if (cpfLinha === cpfLimpo) {
                    cpfDuplicado = true;
                    return false; // break
                }
            });
            if (cpfDuplicado) {
                alert('CPF já existe na lista de beneficiários');
                return;
            }
            }
        
            if (!modoEdicao) {
                // Salva em session via AJAX (inclusão)
                // Envia IdCliente se estiver em modo de alteração de cliente
                var dadosEnvio = {
                    CPF: cpf,
                    Nome: nome
                };
                if (typeof obj !== 'undefined' && obj && obj.Id) {
                    dadosEnvio.IdCliente = obj.Id;
                }
                $.ajax({
                    url: '/Cliente/SalvarBeneficiarioSession',
                    method: 'POST',
                    data: dadosEnvio,
                    success: function(resultado) {
                        if (resultado.success) {
                            // Adiciona linha na tabela com data-cpf para identificação
                            var novaLinha = '<tr data-cpf="' + cpf + '">' +
                                '<td>' + cpf + '</td>' +
                                '<td>' + nome + '</td>' +
                                '<td>' +
                                '<button type="button" class="btn btn-sm btn-primary btn-alterar">Alterar</button> ' +
                                '<button type="button" class="btn btn-sm btn-danger btn-excluir">Excluir</button>' +
                                '</td>' +
                                '</tr>';
                            $('#' + modalAtualId + ' #gridBeneficiarios').append(novaLinha);
                        
                            // Limpa os campos
                        $('#' + modalAtualId + ' #CPF').val('');
                        $('#' + modalAtualId + ' #Nome').val('');
                        $('#' + fbId).hide();
                        includeBtn.prop('disabled', false);
                        
                        alert(resultado.message);
                    }
                },
                error: function(xhr) {
                    var mensagem = 'Erro ao adicionar beneficiário';
                            if (xhr.responseJSON) {
                                mensagem = xhr.responseJSON;
                            }
                            alert(mensagem);
                            console.log('Erro:', xhr);
                        }
                    });
                    } else {
                        // atualização (modo edição)
                        // desabilita botões Alterar/Excluir das linhas enquanto atualiza
                        $('#' + modalAtualId + ' #gridBeneficiarios').find('.btn-alterar, .btn-excluir').prop('disabled', true);

                        // Prepara dados para atualização, incluindo IdCliente se estiver em modo alteração de cliente
                        var dadosAtualizacao = {
                            originalCpf: modoEdicao,
                            CPF: cpf,
                            Nome: nome
                        };
                        if (typeof obj !== 'undefined' && obj && obj.Id) {
                            dadosAtualizacao.IdCliente = obj.Id;
                        }

                        $.ajax({
                            url: '/Cliente/AtualizarBeneficiarioSession',
                            method: 'POST',
                            data: dadosAtualizacao,
                            success: function(resultado) {
                                if (resultado.success) {
                                    // Atualiza linha na tabela
                                    var row = $('#' + modalAtualId + ' #gridBeneficiarios').find('tr[data-cpf="' + modoEdicao + '"]');
                                    if (row.length) {
                                        row.attr('data-cpf', cpf);
                                        row.find('td').eq(0).text(cpf);
                                        row.find('td').eq(1).text(nome);
                                    }

                                    // volta ao estado de inclusão
                                    modoEdicao = null;
                                    includeBtn.text('Incluir');
                        
                                    // Reabilita os botões Alterar/Excluir de todas as linhas
                            $('#' + modalAtualId + ' #gridBeneficiarios').find('.btn-alterar, .btn-excluir').prop('disabled', false);

                        // limpa campos
                        $('#' + modalAtualId + ' #CPF').val('');
                        $('#' + modalAtualId + ' #Nome').val('');
                        $('#' + fbId).hide();

                        alert(resultado.message);
                                        } else {
                                            alert(resultado.message || 'Erro ao atualizar beneficiário');
                                            $('#' + modalAtualId + ' #gridBeneficiarios').find('.btn-alterar, .btn-excluir').prop('disabled', false);
                                        }
                                    },
                                    error: function(xhr) {
                                        alert('Erro ao atualizar beneficiário');
                                        $('#' + modalAtualId + ' #gridBeneficiarios').find('.btn-alterar, .btn-excluir').prop('disabled', false);
                                        console.log('Erro:', xhr);
                                    }
                                });
                            }
                        });

    // Handler para clicar em Alterar em uma linha
    $(document).on('click', '#' + random + ' .btn-alterar', function () {
        var tr = $(this).closest('tr');
        var cpf = tr.find('td').eq(0).text();
        var nome = tr.find('td').eq(1).text();

        // preenche inputs
        $('#' + modalAtualId + ' #CPF').val(cpf);
        $('#' + modalAtualId + ' #Nome').val(nome);

        // muda botão incluir para alterar e armazena cpf original
        modoEdicao = cpf;
        includeBtn.text('Alterar');

        // limpa feedback de CPF e garante botão habilitado (CPF é do próprio beneficiário)
        $('#' + fbId).hide();
        includeBtn.prop('disabled', false);

        // desabilita os botões Alterar/Excluir de todas as linhas enquanto estiver em modo de edição
        $('#' + modalAtualId + ' #gridBeneficiarios').find('.btn-alterar, .btn-excluir').prop('disabled', true);
    });

    // Handler para Excluir
    $(document).on('click', '#' + random + ' .btn-excluir', function () {
        var tr = $(this).closest('tr');
        var cpf = tr.find('td').eq(0).text();

        if (!confirm('Confirma exclusão do beneficiário com CPF ' + cpf + '?')) return;

        $.ajax({
            url: '/Cliente/ExcluirBeneficiarioSession',
            method: 'POST',
            data: { CPF: cpf },
            success: function (res) {
                if (res && res.success) {
                    tr.remove();
                    alert(res.message);
                } else {
                    alert(res.message || 'Erro ao excluir beneficiário');
                }
            },
            error: function () {
                alert('Erro ao excluir beneficiário');
            }
        });
    });
}