namespace ShopNursery.Models
{
    public class PlantModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Price { get; set; } = 0;
        public int Quantity { get; set; } = 0;
        public string Light { get; set;} = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string PortSize { get; set; } = string.Empty;
        public string WaterSchedule { get; set; } = string.Empty;

    }
}
