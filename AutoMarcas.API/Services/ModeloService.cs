using AutoMarcas.API.Models;
using AutoMarcas.API.Repositories.Interfaces;
using AutoMarcas.API.Services.Interfaces;
using DalProLib;

namespace AutoMarcas.API.Services
{
    public class ModeloService : IModeloService
    {
        private readonly IModeloRepository _repo;

        public ModeloService(IModeloRepository repo)
        {
            _repo = repo;
        }

        public List<Modelo> GetAll()
        {
            return _repo.GetAll();
        }

     

        public int Create(string modelDetails)
        {
            return _repo.Create(modelDetails);
        }
    }
}