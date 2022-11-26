namespace CalculadoraSprint.Entidades
{
    public class Tarefa
    {
        public int Codigo  { get; set; }

        public string Descricao { get; set; }

        public int TempoEntrega { get; set; }

        public List<int> TarefasDependentes { get; set; }

        public Pontuacao Pontuacao { get; set; } = new Pontuacao();

        public List<Tarefa> TarefasFilha { get; set; } = new List<Tarefa>();

        public List<Tarefa> TarefasPai { get; set; } = new List<Tarefa>();

    }

    public class Pontuacao
    {
        public int IdaInicio { get; set; }

        public int IdaFim { get; set; }

        public int VoltaInicio { get; set; }

        public int VoltaFim { get; set; }

    }
}
