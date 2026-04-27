using AutoMarcas.API.Models;
using AutoMarcas.API.Repositories.Interfaces;
using DalProLib;

namespace AutoMarcas.API.Repositories
{
    public class MarcaRepository : IMarcaRepository
    {
        public List<Marca> GetAll()
        {
            string sql = "SELECT * FROM Marca";
            return DalPro.Query<Marca>(sql);
        }
        public int Create(string marcaDetails)
        {
            string sql = "INSERT INTO Marca (MarcaDetails) OUTPUT INSERTED.MarcaId VALUES (@MarcaDetails)";
            var parametros = new Dictionary<string, object> { { "MarcaDetails", marcaDetails } };
            return DalPro.Query<int>(sql, parametros).First();
        }
    }
}
