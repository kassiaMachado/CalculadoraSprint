using Microsoft.AspNetCore.Mvc;

namespace CalculadoraSprint.Controllers
{
    [ApiController]

    public class CalculadoraController : ControllerBase
    {

        [HttpPost("calcular_tarefas")]
        public void CalcularTarefas(List<Entidades.Tarefa> tarefas)
        {

            tarefas = CatalogarTarefasFilhas(tarefas);

            tarefas = CatalogarTarefasPai(tarefas);

            CalcularIda(tarefas.Where(x => x.TarefasPai.Count == 0).ToList());
        }

        private List<Entidades.Tarefa> CatalogarTarefasFilhas(List<Entidades.Tarefa> tarefas)
        {
            foreach (var tarefa in tarefas)
            {
                var filhas = tarefas.Where(x => x.TarefasDependentes.Contains(tarefa.Codigo)).ToList();

                tarefa.TarefasFilha.AddRange(filhas);

            }

            return tarefas;
        }

        private List<Entidades.Tarefa> CatalogarTarefasPai(List<Entidades.Tarefa> tarefas)
        {
            foreach (var tarefa in tarefas.OrderByDescending(x => x.Codigo))
            {
                foreach (var codigoPai in tarefa.TarefasDependentes)
                {
                    var pai = tarefas.First(x => x.Codigo == codigoPai);

                    tarefa.TarefasPai.Add(pai);
                }

            }

            return tarefas;
        }

        private void CalcularIda(List<Entidades.Tarefa> tarefas)
        {
            foreach (var tarefa in tarefas)
            {
                if (tarefa.TarefasPai.Count == 0)
                {
                    tarefa.Pontuacao.IdaInicio = 0;
                    tarefa.Pontuacao.IdaFim = tarefa.TempoEntrega;
                }

                else
                {
                    var IdaFimMaximoPai = tarefa.TarefasPai.Max(x => x.TempoEntrega);
                    tarefa.Pontuacao.IdaInicio = IdaFimMaximoPai;
                    tarefa.Pontuacao.IdaFim = tarefa.Pontuacao.IdaInicio + tarefa.TempoEntrega;

                }


                if (tarefa.TarefasFilha.Count > 0)
                    CalcularIda(tarefa.TarefasFilha);
            }

        }

    }
}