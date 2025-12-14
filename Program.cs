using TestProject.Data;
using Microsoft.EntityFrameworkCore;
using TestProject.Services;
using TestProject.Dtos;
using TestProject.Models;
using FluentValidation;
using FluentValidation.Results;
using TestProject.Validators;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options=>
{
    options.UseSqlite("Data Source = products.db");
});
builder.Services.AddScoped<ProductService>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/api/products", async (ProductService service) =>
{
    return await service.GetAllAsync();
});

app.MapGet("/api/products/{id:int}", async (int id, ProductService service) =>
{
    var product = await service.GetByIdAsync(id);
    return product is null
        ? Results.NotFound()
        : Results.Ok(product);
});

app.MapPost("/api/products", async (
    CreateProductDto dto,
    IValidator<CreateProductDto> validator,
    ProductService service) =>
{
    ValidationResult validationResult = await validator.ValidateAsync(dto);

    if (!validationResult.IsValid)
        return Results.BadRequest(validationResult.Errors);

    var product = new Product
    {
        Name = dto.Name,
        Price = dto.Price
    };

    var created = await service.CreateAsync(product);
    return Results.Created($"/api/products/{created.Id}", created);
});


app.MapPut("/api/products/{id:int}", async (
    int id,
    UpdateProductDto dto,
    ProductService service) =>
{
    if (string.IsNullOrWhiteSpace(dto.Name))
        return Results.BadRequest("Name is required");

    if (dto.Price <= 0)
        return Results.BadRequest("Price must be greater than zero");

    var updated = new Product
    {
        Name = dto.Name,
        Price = dto.Price
    };

    var success = await service.UpdateAsync(id, updated);
    return success ? Results.NoContent() : Results.NotFound();
});

app.MapDelete("/api/products/{id:int}", async (
    int id,
    ProductService service) =>
{
    var success = await service.DeleteAsync(id);
    return success ? Results.NoContent() : Results.NotFound();
});

app.Run();