using AutoMarcas.API.DTOs;
using AutoMarcas.API.Repositories.Interfaces;
using AutoMarcas.API.Services.Interfaces;

namespace AutoMarcas.API.Services
{
    public class VeiculoService : IVeiculoService
    {
        private readonly IVeiculoRepository _repo;

        public VeiculoService(IVeiculoRepository repo)
        {
            _repo = repo;
        }

        public List<VeiculoDTO> GetAll()
        {
            return _repo.GetAll();
        }

        public VeiculoDTO? GetById(int id)
        {
            return _repo.GetById(id);
        }

        public int Create(VeiculoCreateDTO dto)
        {
            return _repo.Insert(dto);
        }

        public bool Update(int id, VeiculoCreateDTO dto)
        {
            return _repo.Update(id, dto);
        }

        public bool Delete(int id)
        {
            return _repo.Delete(id);
        }
    }
}