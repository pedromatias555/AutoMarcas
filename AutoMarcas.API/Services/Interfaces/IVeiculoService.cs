using AutoMarcas.API.DTOs;

namespace AutoMarcas.API.Services.Interfaces
{
    public interface IVeiculoService
    {
        List<VeiculoDTO> GetAll();
        VeiculoDTO? GetById(int id);
        int Create(VeiculoCreateDTO dto);
        bool Update(int id, VeiculoCreateDTO dto);
        bool Delete(int id);
    }
}