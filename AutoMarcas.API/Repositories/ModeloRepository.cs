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
    }
}

