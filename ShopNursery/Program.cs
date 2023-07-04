using DotNet.NLogger.NetCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NLog.Config;
using ShopNursery;
using ShopNursery.Models;
using ShopNursery.PlantDbContext;
using ShopNursery.Repository;
using ShopNursery.Services;

var builder = WebApplication.CreateBuilder(args);
//builder.Host.ConfigureLogging(ConfigureServices.AppConfigure);

builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<PlantContext>(opt => opt.UseInMemoryDatabase("PlantsShop"));
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ILoggerManager,LoggerService>();
builder.Services.AddScoped<IPlantService, PlantService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseSession();


app.MapGet("/GetPlants", ([FromServices] IPlantService _plant , CancellationToken cancellation) =>
{
    return _plant.GetPlants(cancellation); 
}
);
app.MapGet("/GetPlantsById/{id}", ([FromServices] IPlantService _plant,int id, CancellationToken cancellation) =>
{
    return _plant.GetPlantById(id, cancellation);
}
);
app.MapPost("/CreatePlant", async ([FromServices] IPlantService _plant, [FromBody] PlantModel plant  , CancellationToken cancellation) =>
{
    var result = await _plant.CreatePlant(plant ,cancellation);
    return new OkObjectResult(new { result = "Plant Created with id : " + result});
}
);
app.MapPost("/UpdatePlant/{id}", async ([FromServices] IPlantService _plant, [FromBody] PlantModel plant , [FromQuery] int id, CancellationToken cancellation) =>
{
    var result = await _plant.UpdatePlant(plant , id, cancellation);
    return result ? Results.Content("Updated Successfully") : Results.NotFound();
}
);
app.MapDelete("/DeletePlant/{id}", async ([FromServices] IPlantService _plant,[FromQuery] int id, CancellationToken cancellation) =>
{
    var result = await _plant.DeletePlant(id , cancellation);
    return result ? Results.Content("Deleted Successfully") : Results.NotFound();
}
);
app.MapPost("/AddtoCart", async ([FromServices] IPlantService _plant, [FromBody] AddToCartModel cart, CancellationToken cancellation) =>
{
    var result = await _plant.AddToCart(cart , cancellation);
    return Results.Content(result);
}
);
app.MapGet("/GetCart", async ([FromServices] IPlantService _plant, CancellationToken cancellation) =>
{
    var result = await _plant.GetCart(cancellation);
    return result;
}
);
app.Run();


