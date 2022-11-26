using Microsoft.AspNetCore.Mvc;

namespace CalculadoraSprint.Controllers
{
    [ApiController]

    public class CalculadoraController : ControllerBase
    {

        [HttpPost("calcular_tarefas")]
        public void CalcularTarefas(List<Entidades.Tarefa> tarefas)
        {

            tarefas=CatalogarTarefasFilhas(tarefas);
            
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

        private 



    }
}