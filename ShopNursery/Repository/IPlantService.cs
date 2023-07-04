using ShopNursery.Models;

namespace ShopNursery.Repository
{
    public interface IPlantService
    {
        Task<int> CreatePlant(PlantModel plant , CancellationToken cancellationToken);

        Task<bool> UpdatePlant(PlantModel plant , int plantId, CancellationToken cancellationToken);

        Task<bool> DeletePlant(int plantId , CancellationToken cancellationToken);

        Task<List<PlantModel>> GetPlants(CancellationToken cancellation);

        Task<PlantModel> GetPlantById(int plantId, CancellationToken cancellationToken);

        Task<string> AddToCart(AddToCartModel cart , CancellationToken cancellationToken);

        Task<List<AddToCartModel>> GetCart(CancellationToken cancellationToken);
    }
}
