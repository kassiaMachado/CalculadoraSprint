using Microsoft.AspNetCore.Mvc;

namespace CalculadoraSprint.Controllers
{
    [ApiController]
    
    public class CalculadoraController : ControllerBase
    {

        [HttpPost ("calcular_tarefas")]
        public void CalcularTarefas(List<Entidades.Tarefa> tarefas)
        {
            
        }
    }
}