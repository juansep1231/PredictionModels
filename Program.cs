using DashboardModels;
using MemoriaMicro;
using Microsoft.Extensions.ML;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddPredictionEnginePool<MLModelAccesosBancaMovil.ModelInput, MLModelAccesosBancaMovil.ModelOutput>()
    .FromFile("MLModelAccesosBancaMovil.mlnet");

builder.Services.AddPredictionEnginePool<MLModelTarjetasDebito.ModelInput, MLModelTarjetasDebito.ModelOutput>()
    .FromFile("MLModelTarjetasDebito.mlnet"); 

//MODELOS FER
builder.Services.AddPredictionEnginePool<MLModel1BFF.ModelInput, MLModel1BFF.ModelOutput>()
    .FromFile("MLModel1BFF.mlnet");

builder.Services.AddPredictionEnginePool<MLModelMirco2.ModelInput, MLModelMirco2.ModelOutput>()
    .FromFile("MLModelMirco2.mlnet");

builder.Services.AddPredictionEnginePool<MLModelAccesos_Produnet.ModelInput, MLModelAccesos_Produnet.ModelOutput>()
    .FromFile("MLModelAccesos_Produnet.mlnet");

builder.Services.AddPredictionEnginePool<MLModel_MemoriaBFF.ModelInput, MLModel_MemoriaBFF.ModelOutput>()
    .FromFile("MLModel_MemoriaBFF.mlnet");

builder.Services.AddPredictionEnginePool<MLModelMemoria_Micros.ModelInput, MLModelMemoria_Micros.ModelOutput>()
    .FromFile("MLModelMemoria_Micros.mlnet");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Description = "Docs for my API",
        Version = "v1"
    });

    // Configuraci�n para evitar conflictos de nombres en Swagger
    c.CustomSchemaIds(type => type.FullName.Replace("+", "."));

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll"); // Habilita CORS usando la pol�tica definida

app.UseAuthorization();

app.MapControllers();

app.Run();
