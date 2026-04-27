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
    }
}
