using AutoMarcas.API.Models;
using AutoMarcas.API.Repositories.Interfaces;
using DalProLib;

namespace AutoMarcas.API.Repositories
{
    public class ModeloRepository : IModeloRepository
    {
        public List<Modelo> GetAll()
        {
            string sql = "SELECT * FROM Modelo";
            return DalPro.Query<Modelo>(sql);
        }

        public int Create(string modelDetails)
        {
            string sql = "INSERT INTO Modelo (ModelDetails) OUTPUT INSERTED.ModeloId VALUES (@ModelDetails)";
            var parametros = new Dictionary<string, object> { { "ModelDetails", modelDetails } };
            return DalPro.Query<int>(sql, parametros).First();
        }
    }
}

