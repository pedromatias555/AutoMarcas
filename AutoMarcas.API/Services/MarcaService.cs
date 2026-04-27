using AutoMarcas.API.Models;
using AutoMarcas.API.Repositories.Interfaces;
using AutoMarcas.API.Services.Interfaces;

namespace AutoMarcas.API.Services
{
    public class MarcaService : IMarcaService
    {
        private readonly IMarcaRepository _repo;

        public MarcaService(IMarcaRepository repo)
        {
            _repo = repo;
        }

        public List<Marca> GetAll()
        {
            return _repo.GetAll();
        }
    }
}