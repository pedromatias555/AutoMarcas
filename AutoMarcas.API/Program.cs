using AutoMarcas.API.Repositories;
using AutoMarcas.API.Repositories.Interfaces;
using AutoMarcas.API.Services;
using AutoMarcas.API.Services.Interfaces;
using AutoMarcas.API.DTOs;
using DalProLib;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("cors", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Injeção de Dependências
builder.Services.AddScoped<IMarcaRepository, MarcaRepository>();
builder.Services.AddScoped<IModeloRepository, ModeloRepository>();
builder.Services.AddScoped<IVeiculoRepository, VeiculoRepository>();
builder.Services.AddScoped<IMarcaService, MarcaService>();
builder.Services.AddScoped<IModeloService, ModeloService>();
builder.Services.AddScoped<IVeiculoService, VeiculoService>();

var app = builder.Build();

// Connection string da DalPro
DalPro.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

app.UseCors("cors");
app.UseSwagger();
app.UseSwaggerUI();

// Endpoints Marca
app.MapGet("/marcas", (IMarcaService service) =>
{
    return service.GetAll();
});

// Endpoints Modelo
app.MapGet("/modelos", (IModeloService service) =>
{
    return service.GetAll();
});

// Endpoints Veiculo
app.MapGet("/veiculos", (IVeiculoService service) =>
{
    return service.GetAll();
});

app.MapGet("/veiculos/{id}", (int id, IVeiculoService service) =>
{
    var v = service.GetById(id);
    return v == null ? Results.NotFound() : Results.Ok(v);
});

app.MapPost("/veiculos", (VeiculoCreateDTO dto, IVeiculoService service) =>
{
    int id = service.Create(dto);
    return Results.Created($"/veiculos/{id}", new { id });
});

app.MapPut("/veiculos/{id}", (int id, VeiculoCreateDTO dto, IVeiculoService service) =>
{
    service.Update(id, dto);
    return Results.Ok();
});

app.MapDelete("/veiculos/{id}", (int id, IVeiculoService service) =>
{
    service.Delete(id);
    return Results.Ok();
});

app.MapPost("/marcas", (string marcaDetails, IMarcaService service) =>
{
    int id = service.Create(marcaDetails);
    return Results.Created($"/marcas/{id}", new { marcaId = id, marcaDetails });
});

app.MapPost("/modelos", (string modelDetails, IModeloService service) =>
{
    int id = service.Create(modelDetails);
    return Results.Created($"/modelos/{id}", new { modeloId = id, modelDetails });
});

app.Run();