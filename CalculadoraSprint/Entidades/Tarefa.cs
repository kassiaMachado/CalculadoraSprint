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

        
    }

    public class Pontuacao
    {
        public int InicioMaisAntigo { get; set; }

        public int InicioMaisRecente { get; set; }

        public int PrimeiroFim { get; set; }

        public int UltimoFim { get; set; }

    }
}
