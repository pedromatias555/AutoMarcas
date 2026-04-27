using AutoMarcas.API.DTOs;
using AutoMarcas.API.Repositories.Interfaces;
using DalProLib;

namespace AutoMarcas.API.Repositories
{
    public class VeiculoRepository : IVeiculoRepository
    {
        public List<VeiculoDTO> GetAll()
        {
            string sql = @"
                SELECT v.VeiculoId, v.Ano, v.Vendido, v.UltimaInspecao,
                       v.MarcaId, m.MarcaDetails,
                       v.ModeloId, mo.ModelDetails,
                       v.TipoId, t.Tipologia
                FROM Veiculo v
                INNER JOIN Marca m ON m.MarcaId = v.MarcaId
                INNER JOIN Modelo mo ON mo.ModeloId = v.ModeloId
                INNER JOIN TipoVeiculo t ON t.TipoId = v.TipoId";

            return DalPro.Query<VeiculoDTO>(sql);
        }

        public VeiculoDTO? GetById(int id)
        {
            string sql = @"
                SELECT v.VeiculoId, v.Ano, v.Vendido, v.UltimaInspecao,
                       v.MarcaId, m.MarcaDetails,
                       v.ModeloId, mo.ModelDetails,
                       v.TipoId, t.Tipologia
                FROM Veiculo v
                INNER JOIN Marca m ON m.MarcaId = v.MarcaId
                INNER JOIN Modelo mo ON mo.ModeloId = v.ModeloId
                INNER JOIN TipoVeiculo t ON t.TipoId = v.TipoId
                WHERE v.VeiculoId = @id";

            var param = new Dictionary<string, object> { { "@id", id } };
            return DalPro.Query<VeiculoDTO>(sql, param).FirstOrDefault();
        }

        public int Insert(VeiculoCreateDTO dto)
        {
            string sql = @"
                INSERT INTO Veiculo (Ano, Vendido, UltimaInspecao, TipoId, ModeloId, MarcaId)
                VALUES (@Ano, @Vendido, @UltimaInspecao, @TipoId, @ModeloId, @MarcaId);
                SELECT SCOPE_IDENTITY();";

            var param = new Dictionary<string, object>
            {
                { "@Ano", dto.Ano },
                { "@Vendido", dto.Vendido },
                { "@UltimaInspecao", dto.UltimaInspecao ?? (object)DBNull.Value },
                { "@TipoId", dto.TipoId },
                { "@ModeloId", dto.ModeloId },
                { "@MarcaId", dto.MarcaId }
            };

            return Convert.ToInt32(DalPro.ExecuteScalar(sql, param));
        }

        public bool Update(int id, VeiculoCreateDTO dto)
        {
            string sql = @"
                UPDATE Veiculo SET
                    Ano = @Ano,
                    Vendido = @Vendido,
                    UltimaInspecao = @UltimaInspecao,
                    TipoId = @TipoId,
                    ModeloId = @ModeloId,
                    MarcaId = @MarcaId
                WHERE VeiculoId = @id";

            var param = new Dictionary<string, object>
            {
                { "@Ano", dto.Ano },
                { "@Vendido", dto.Vendido },
                { "@UltimaInspecao", dto.UltimaInspecao ?? (object)DBNull.Value },
                { "@TipoId", dto.TipoId },
                { "@ModeloId", dto.ModeloId },
                { "@MarcaId", dto.MarcaId },
                { "@id", id }
            };

            return DalPro.Execute(sql, param) > 0;
        }

        public bool Delete(int id)
        {
            string sql = "DELETE FROM Veiculo WHERE VeiculoId = @id";
            var param = new Dictionary<string, object> { { "@id", id } };
            return DalPro.Execute(sql, param) > 0;
        }
    }
}