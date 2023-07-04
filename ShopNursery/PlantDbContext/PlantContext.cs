using Microsoft.EntityFrameworkCore;
using ShopNursery.Models;

namespace ShopNursery.PlantDbContext
{
    public class PlantContext : DbContext
    {
        public PlantContext(DbContextOptions<PlantContext> options) : base(options)
        {
            
        }

        public DbSet<PlantModel> plants => Set<PlantModel>();
    }
}
