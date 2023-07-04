using Microsoft.EntityFrameworkCore;
using ShopNursery.Models;
using ShopNursery.PlantDbContext;
using ShopNursery.Repository;
using ShopNursery.Utils;
using System.Web;

namespace ShopNursery.Services
{
    public class PlantService : IPlantService
    {
        const string SessionName = "PlantCart";
        private readonly IHttpContextAccessor contextAccessor;
        private readonly PlantContext dbContext;
        private static List<PlantModel> Plants;
        private readonly ILoggerManager _logger;
        public PlantService(IHttpContextAccessor _context, PlantContext _dbContext, ILoggerManager logger)
        {
            Plants = new List<PlantModel>();
            contextAccessor = _context;
            this.dbContext = _dbContext;
            _logger = logger;

        }
        public async Task<int> CreatePlant(PlantModel plant, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Called : {nameof(CreatePlant)}");
            if (plant.Id is null)
            {
                plant.Id = await dbContext.plants.CountAsync() + 1;
                dbContext.plants.Add(plant);
                await dbContext.SaveChangesAsync();
                _logger.LogInfo($"Id : {plant.Id} is created.");
                return (int)plant.Id;
            }
            else
            {
                return 0;
            }


        }

        public async Task<bool> DeletePlant(int plantId, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Called : {nameof(DeletePlant)}");

            if (await dbContext.plants.FindAsync(plantId) is PlantModel plant)
            {
                dbContext.plants.Remove(plant);
                await dbContext.SaveChangesAsync();
                _logger.LogInfo($"Id : {plant.Id} is deleted.");
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<PlantModel> GetPlantById(int plantId, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Called : {nameof(GetPlantById)} with Id : {plantId}");
            return (PlantModel)(await dbContext.plants.FindAsync(plantId) is PlantModel plant ? plant : new PlantModel());
        }

        public async Task<List<PlantModel>> GetPlants(CancellationToken cancellation)
        {
            _logger.LogInfo($"Called : {nameof(GetPlants)}");
            return await dbContext.plants.ToListAsync();
        }

        public async Task<bool> UpdatePlant(PlantModel plant, int plantId, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Called : {nameof(UpdatePlant)} with id : {plantId}");
            var updatePlant = await dbContext.plants.FindAsync(plantId);
            if (updatePlant is null) { return false; }
            updatePlant.Name = plant.Name;
            updatePlant.Description = plant.Description;
            updatePlant.Location = plant.Location;
            updatePlant.Price = plant.Price;
            updatePlant.PortSize = plant.PortSize;
            updatePlant.WaterSchedule = plant.WaterSchedule;
            updatePlant.Quantity = plant.Quantity;
            updatePlant.Light = plant.Light;
            updatePlant.Type = plant.Type;
            await dbContext.SaveChangesAsync();
            _logger.LogInfo($"Id : {plantId} is updated.");
            return true;
        }

        public async Task<string> AddToCart(AddToCartModel cart, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Called : {nameof(AddToCart)}");
            var orderedPlant = await dbContext.plants.FindAsync(cart.Id);
            if (orderedPlant is not null && orderedPlant.Quantity > 0 && (orderedPlant.Quantity - cart.Quantity) >= 0)
            {
                List<AddToCartModel> currentCart;
                if (contextAccessor.HttpContext?.Session.Get<AddToCartModel>(SessionName) == default)
                {
                    currentCart = new List<AddToCartModel> { cart };
                    contextAccessor.HttpContext?.Session.Set<AddToCartModel>(SessionName, (IEnumerable<AddToCartModel>)currentCart);
                }
                else
                {
                    currentCart = contextAccessor.HttpContext?.Session.Get<AddToCartModel>(SessionName).ToList();
                    if (currentCart.Any(Item => Item.Id == cart.Id))
                    {
                        currentCart.Where(Item => Item.Id == cart.Id).Select(S => { S.Quantity = (S.Quantity + cart.Quantity); return S; }).ToList();
                    }
                    else
                    {
                        currentCart.Add(cart);
                    }
                    contextAccessor.HttpContext?.Session.Set<AddToCartModel>(SessionName, (IEnumerable<AddToCartModel>)currentCart);
                }
                orderedPlant.Quantity -= cart.Quantity;
                await dbContext.SaveChangesAsync();
                _logger.LogInfo($"Added to Cart : PlantID : {cart.Id} , Quantity : {cart.Quantity}");
                return "Added to Cart";
            }
            else
            {
                if (orderedPlant is null)
                {
                    _logger.LogInfo($"Plant Id not found id : {cart.Id}");
                    return "PLant not found";
                }
                else if (orderedPlant.Quantity <= 0)
                {
                    _logger.LogInfo($"Plant Id : {cart.Id} is out of Stock!..  ");
                    return "Out of Stock!";
                }
                else if ((orderedPlant.Quantity - cart.Quantity) < 0)
                {
                    _logger.LogInfo($"Plant Id : {cart.Id} , Only : {orderedPlant.Quantity} avilable!..  ");
                    return "Avialable Stock  : " + orderedPlant.Quantity;
                }
            }
            throw new Exception("Something Went Wrong");
        }

        public Task<List<AddToCartModel>> GetCart(CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Called : {nameof(GetCart)}");
            if (contextAccessor.HttpContext?.Session.Get<AddToCartModel>(SessionName) == default)
            {
                return Task.FromResult(new List<AddToCartModel>());
            }
            else
            {
                return Task.FromResult(contextAccessor.HttpContext?.Session.Get<AddToCartModel>(SessionName).ToList());
            }
        }
    }
}
