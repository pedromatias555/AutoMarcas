using AutoMarcas.API.Models;

namespace AutoMarcas.API.Repositories.Interfaces
{
    public interface IMarcaRepository
    {
        List<Marca> GetAll();
    }
}