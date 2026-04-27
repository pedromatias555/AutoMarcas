namespace AutoMarcas.API.Models
{
    public class Veiculo
    {
        public int VeiculoId { get; set; }
        public int Ano { get; set; }
        public bool Vendido { get; set; }
        public DateTime? UltimaInspecao { get; set; }
        public int TipoId { get; set; }
        public int ModeloId { get; set; }
        public int MarcaId { get; set; }
    }
}