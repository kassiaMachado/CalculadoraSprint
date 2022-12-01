 var codigo = 0;
        var table;
        $(document).ready(function () {
            table = $('#tabelaTarefas').DataTable({
                language: {
                    url: 'https://cdn.datatables.net/plug-ins/1.13.1/i18n/pt-BR.json'
                }
            });

            $('#tabelaTarefas tbody').on('click', 'button', function () {
                var conteudoLinha = table.row($(this).parents('tr')).data();
                var codigoTarefa = conteudoLinha[0];
                var isDependente = VerificarDependentes(codigoTarefa);

                if (isDependente) {
                    Swal.fire({
                        icon: 'warning',
                        title: 'Oops...',
                        text: "Não é possível excluir uma tarefa que possui dependentes."
                    });
                } else {
                    table.row($(this).parents('tr')).remove().draw();
                    EnviarParaApi();
                }
            });
        });

        function CadastrarTarefa() {

            var novoCodigoTarefa = codigo + 1;

            //verificar se existe as tarefas se tiver dependentes
            if ($("#tarefasDependentes").val() != "") {
                var isTemTodosDependentes = VerificarSeExisteTarefasDependentes($("#tarefasDependentes").val().split(","));
                if (!isTemTodosDependentes) {
                    Swal.fire({
                        icon: 'warning',
                        title: 'Oops...',
                        text: "Não é possível cadastrar a tarefa, pois não existe tarefa dependente correspondente a informada."
                    });
                    return;
                }
            }

            codigo = novoCodigoTarefa;
            table.row.add([
                codigo,
                $("#nomeTarefa").val(),
                $("#tempoTarefa").val(),
                $("#tarefasDependentes").val(),
                "",
                "<button type=\"button\" class=\"btn btn-danger btn-floating\"><i class=\"fas fa-trash\"></i></button>"
            ]).draw();
            EnviarParaApi();
            $("#formTarefa").trigger("reset");
        }

        function VerificarDependentes(codigoTarefa) {
            var linhasTabela = table.rows().data();

            var resultado = false;

            $.each(linhasTabela, function (numeroLinha, conteudoLinha) {
                var arrTarefasDependentes = conteudoLinha[3].split(",");

                $.each(arrTarefasDependentes, function (indexDependente, identificadorTarefaDependente) {
                    if (parseInt(identificadorTarefaDependente) == parseInt(codigoTarefa)) {
                        resultado = true;
                    }
                });

            });

            return resultado;
        }

        function VerificarSeExisteTarefasDependentes(arrDependentes) {
            var linhasTabela = table.rows().data();

            var resultado = false;

            $.each(arrDependentes, function (nDependente, rDependente) {
                resultado = false;

                $.each(linhasTabela, function (nTabela, rTabela) {
                    if (parseInt(rTabela[0]) == parseInt(rDependente)) {
                        resultado = true;
                    }
                });

                if (resultado == false) {
                    return false;
                }
            });

            return resultado;
        }

        function CriarObjetoJson() {
            var arrObj = [];
            var linhasTabela = table.rows().data();
            $.each(linhasTabela, function (nTabela, rTabela) {
                var obj = {
                    codigo: parseInt(rTabela[0]),
                    descricao: rTabela[1],
                    tempoEntrega: parseInt(rTabela[2]),
                    tarefasDependentes: []
                };

                if (rTabela[3] != '') {
                    var tarefasDependentes = rTabela[3].split(",");
                    $.each(tarefasDependentes, function (tIndex, tContent) {
                        obj.tarefasDependentes.push(parseInt(tContent));
                    });
                }

                arrObj.push(obj);
            });

            return JSON.stringify(arrObj);
        }

        function EnviarParaApi() {
            if (linhasTabela = table.rows().data().length > 0) {
                var obj = CriarObjetoJson();

                $.ajax({
                    url: "/calcular_tarefas",
                    method: "POST",
                    dataType: "json",
                    contentType: "application/json",
                    data: obj
                }).done(function (r) {
                    $("#prazoMaximo").html(r.prazoMaximo + " dia(s)");
                    $("#caminhoCritico").html(r.caminhoCritico);
                    AtualizarLinhaComResultado(r.pontuacoes);
                }).fail(function (xhr, err, stack) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Oops...',
                        text: 'Algo deu errado ao enviar o calculo para a API!' + xhr.responseText
                    });
                });
            } else {
                $("#prazoMaximo").html("A CALCULAR");
                $("#caminhoCritico").html("A CALCULAR");
            }
        }

        function AtualizarLinhaComResultado(pontuacoes) {
            $.each(pontuacoes, function (pIndex, pContent) {
                $.each(table.rows().data(), function (tIndex, tContent) {
                    if (tContent[0] == pContent.codigoTarefa) {
                        var resultado = "Ida Início: " + pContent.idaInicio + " dias<br>Ida Fim: " + pContent.idaFim + " dias<br>Volta Início: " + pContent.voltaInicio + " dias<br>Volta Fim: " + pContent.voltaFim + " dias";
                        tContent[4] = resultado;
                        table.row(tIndex).data(tContent).draw();
                    }
                });
            });
        }

        $("#formTarefa").submit(function (e) {
            e.preventDefault();
            CadastrarTarefa();
            $("#nomeTarefa").focus();
        });