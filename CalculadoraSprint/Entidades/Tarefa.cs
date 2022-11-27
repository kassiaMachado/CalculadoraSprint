namespace CalculadoraSprint.Entidades
{
    public class Tarefa
    {
        public int Codigo  { get; set; }

        public string Descricao { get; set; }

        public int TempoEntrega { get; set; }

        public List<int> TarefasDependentes { get; set; }

        // sucessoras da tarefa
        public List<Tarefa> TarefasFilha { get; set; } = new List<Tarefa>();
        // predecessor da tarefa
        public List<Tarefa> TarefasPai { get; set; } = new List<Tarefa>();

    }

    public class Pontuacao
    {
        public int CodigoTarefa { get; set; }
        public int IdaInicio { get; set; }

        public int IdaFim { get; set; }

        public int VoltaInicio { get; set; }

        public int VoltaFim { get; set; }

    }
}
