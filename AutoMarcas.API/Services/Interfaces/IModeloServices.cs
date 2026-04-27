using AutoMarcas.API.Models;

namespace AutoMarcas.API.Services.Interfaces
{
    public interface IModeloService
    {
        List<Modelo> GetAll();

        int Create(string modelDetails);

    }

}