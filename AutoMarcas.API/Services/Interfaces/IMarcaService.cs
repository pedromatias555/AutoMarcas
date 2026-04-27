using AutoMarcas.API.Models;

namespace AutoMarcas.API.Services.Interfaces
{
    public interface IMarcaService
    {
        List<Marca> GetAll();
    }
}