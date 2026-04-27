using AutoMarcas.API.Models;

namespace AutoMarcas.API.Repositories.Interfaces
{
    public interface IModeloRepository
    {
        List<Modelo> GetAll();

        int Create(string modelDetails);
    }
}