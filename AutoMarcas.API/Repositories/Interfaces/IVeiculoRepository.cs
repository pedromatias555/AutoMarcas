using AutoMarcas.API.DTOs;

namespace AutoMarcas.API.Repositories.Interfaces
{
    public interface IVeiculoRepository
    {
        List<VeiculoDTO> GetAll();
        VeiculoDTO? GetById(int id);
        int Insert(VeiculoCreateDTO dto);
        bool Update(int id, VeiculoCreateDTO dto);
        bool Delete(int id);
    }
}