namespace AutoMarcas.API.DTOs
{
    public class VeiculoDTO
    {
        public int VeiculoId { get; set; }
        public int Ano { get; set; }
        public bool Vendido { get; set; }
        public DateTime? UltimaInspecao { get; set; }
        public int TipoId { get; set; }
        public string Tipologia { get; set; }
        public int ModeloId { get; set; }
        public string ModelDetails { get; set; }
        public int MarcaId { get; set; }
        public string MarcaDetails { get; set; }
    }
}