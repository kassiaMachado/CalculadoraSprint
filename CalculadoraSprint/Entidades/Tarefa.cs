namespace CalculadoraSprint.Entidades
{
    public class Tarefa
    {
        public int Codigo  { get; set; }

        public string Descricao { get; set; }

        public int TempoEntrega { get; set; }

        public int? CodigoTarefaDependente { get; set; }
    }
}
