using CalculadoraSprint.Entidades;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CalculadoraSprint.Controllers
{
    [ApiController]

    public class CalculadoraController : ControllerBase
    {
        private List<List<int>> _bateriasCalculo = new List<List<int>>();
        // codigoTarefa, classe pontuacao
        private List<Pontuacao> PontuacaoTarefas { get; set; } = new List<Pontuacao>();
        private int PontuacaoFim { get; set; }

        [HttpPost("calcular_tarefas")]
        public IActionResult CalcularTarefas(List<Tarefa> tarefas)
        {
            CatalogarTarefasFilhas(tarefas);
            CatalogarTarefasPai(tarefas);
            GerarBateriasCalculo(tarefas.Where(x => x.TarefasPai.Count == 0).ToList());
            CalcularIda(tarefas);
            ObterPontuacaoFim(tarefas);
            CalcularVolta(tarefas);
            CalcularFolgaTotal();
            _bateriasCalculo.Reverse();
            var pontuacoesIgualZero = PontuacaoTarefas.Where(x => _bateriasCalculo[0].Any(z => z ==x.CodigoTarefa)
            && x.FolgaTotal == 0).ToList();


            return Ok(new Resultado()
            {
                Pontuacoes = PontuacaoTarefas,
                PrazoMaximo = PontuacaoFim
                 
            });
            

        }

        private void CatalogarTarefasFilhas(List<Tarefa> tarefas)
        {
            foreach (var tarefa in tarefas)
            {
                var filhas = tarefas.Where(x => x.TarefasDependentes.Contains(tarefa.Codigo)).ToList();
                tarefa.TarefasFilha.AddRange(filhas);
            }
        }

        private void CatalogarTarefasPai(List<Tarefa> tarefas)
        {
            foreach (var tarefa in tarefas.OrderByDescending(x => x.Codigo))
            {
                foreach (var codigoPai in tarefa.TarefasDependentes)
                {
                    var pai = tarefas.First(x => x.Codigo == codigoPai);
                    tarefa.TarefasPai.Add(pai);
                }
            }
        }

        private void GerarBateriasCalculo(List<Tarefa> tarefas)
        {
            var bateria = new List<int>();
            foreach (var tarefa in tarefas)
                bateria.Add(tarefa.Codigo);
            _bateriasCalculo.Add(bateria);


            var filhas = new List<Tarefa>();
            foreach (var tarefa in tarefas.Where(x => x.TarefasFilha.Count > 0).ToList())
            {
                filhas.AddRange(tarefa.TarefasFilha);
            }
            filhas = filhas.Distinct().ToList();
            if (filhas.Count > 0)
                GerarBateriasCalculo(filhas);
        }

        private void CalcularIda(List<Tarefa> tarefas)
        {
            foreach (var tarefasCalcular in _bateriasCalculo)
            {
                foreach (var tarefaCalcular in tarefasCalcular)
                {
                    var tarefa = tarefas.First(x => x.Codigo == tarefaCalcular);
                    var pontuacao = new Pontuacao
                    {
                        CodigoTarefa = tarefa.Codigo
                    };

                    if (tarefa.TarefasPai.Count == 0)
                    {
                        pontuacao.IdaInicio = 0;
                        pontuacao.IdaFim = tarefa.TempoEntrega;
                    }
                    else
                    {
                        var maxAntecessoras = PontuacaoTarefas.Where(x => tarefa.TarefasDependentes.Contains(x.CodigoTarefa)).Max(x => x.IdaFim);
                        pontuacao.IdaInicio = maxAntecessoras;
                        pontuacao.IdaFim = pontuacao.IdaInicio + tarefa.TempoEntrega;
                    }

                    if (!PontuacaoTarefas.Any(x => x.CodigoTarefa == tarefa.Codigo))
                        PontuacaoTarefas.Add(pontuacao);
                }
            }
        }

        private void ObterPontuacaoFim(List<Tarefa> tarefas)
        {
            var tarefasFim = tarefas.Where(x => x.TarefasFilha.Count == 0).ToList();
            var fim = PontuacaoTarefas.Where(x => tarefasFim.Any(z => z.Codigo == x.CodigoTarefa)).ToList();
            PontuacaoFim = fim.OrderByDescending(x => x.IdaFim).First().IdaFim;
        }

        private void CalcularVolta(List<Tarefa> tarefas)
        {
            _bateriasCalculo.Reverse();
            foreach (var tarefasCalcular in _bateriasCalculo)
            {
                foreach (var tarefaCalcular in tarefasCalcular)
                {
                    var tarefa = tarefas.First(x => x.Codigo == tarefaCalcular);
                    var pontuacao = PontuacaoTarefas.First(x => x.CodigoTarefa == tarefa.Codigo);

                    if (tarefa.TarefasFilha.Count == 0)
                    {
                        pontuacao.VoltaFim = PontuacaoFim;
                        pontuacao.VoltaInicio = pontuacao.VoltaFim - tarefa.TempoEntrega;
                    }
                    else
                    {
                        if (tarefa.TarefasFilha.Count > 0)
                        {
                            var maxPredecessoras = PontuacaoTarefas.Where(x => tarefa.TarefasFilha.Any(z => z.Codigo == x.CodigoTarefa)).Min(x => x.VoltaInicio);
                            pontuacao.VoltaFim = maxPredecessoras;
                            pontuacao.VoltaInicio = pontuacao.VoltaFim - tarefa.TempoEntrega;
                        }
                    }
                }
            }
        }

        private void CalcularFolgaTotal()
        {
            foreach (var item in PontuacaoTarefas)
            {
                item.FolgaTotal = item.VoltaFim - item.IdaFim;
            }
        }

        private void CaminhoCritico(List<Pontuacao> pontuacoes)
        {
            //foreach 
        }

    }
}