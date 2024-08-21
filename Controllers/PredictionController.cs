using DashboardModels;
using DashboardModels.Controllers;
using MemoriaMicro;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;

using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class PredictionController : ControllerBase
{
    private readonly PredictionEnginePool<MLModelAccesosBancaMovil.ModelInput, MLModelAccesosBancaMovil.ModelOutput> _predictionEnginePool1; //J
    private readonly PredictionEnginePool<MLModelTarjetasDebito.ModelInput, MLModelTarjetasDebito.ModelOutput> _predictionEnginePool2; //J
    private readonly PredictionEnginePool<MLModel1BFF.ModelInput, MLModel1BFF.ModelOutput> _predictionEnginePool3; //F
    private readonly PredictionEnginePool<MLModelMirco2.ModelInput, MLModelMirco2.ModelOutput> _predictionEnginePool4; //F
    private readonly PredictionEnginePool<MLModelAccesos_Produnet.ModelInput, MLModelAccesos_Produnet.ModelOutput> _predictionEnginePool5; //K
    private readonly PredictionEnginePool<MLModel_MemoriaBFF.ModelInput, MLModel_MemoriaBFF.ModelOutput> _predictionEnginePool6; //D
    private readonly PredictionEnginePool<MLModelMemoria_Micros.ModelInput, MLModelMemoria_Micros.ModelOutput> _predictionEnginePool7; //K
    public PredictionController(
        PredictionEnginePool<MLModelAccesosBancaMovil.ModelInput, MLModelAccesosBancaMovil.ModelOutput> predictionEnginePool1,
        PredictionEnginePool<MLModelTarjetasDebito.ModelInput, MLModelTarjetasDebito.ModelOutput> predictionEnginePool2,
        PredictionEnginePool<MLModel1BFF.ModelInput, MLModel1BFF.ModelOutput> predictionEnginePool3,
        PredictionEnginePool<MLModelMirco2.ModelInput, MLModelMirco2.ModelOutput> predictionEnginePool4,
        PredictionEnginePool<MLModelAccesos_Produnet.ModelInput, MLModelAccesos_Produnet.ModelOutput> predictionEnginePool5,
        PredictionEnginePool<MLModel_MemoriaBFF.ModelInput, MLModel_MemoriaBFF.ModelOutput> predictionEnginePool6,
        PredictionEnginePool<MLModelMemoria_Micros.ModelInput, MLModelMemoria_Micros.ModelOutput> predictionEnginePool7

        )
    {
        _predictionEnginePool1 = predictionEnginePool1;
        _predictionEnginePool2 = predictionEnginePool2;
        _predictionEnginePool3 = predictionEnginePool3;
        _predictionEnginePool4 = predictionEnginePool4;
        _predictionEnginePool5 = predictionEnginePool5;
        _predictionEnginePool6 = predictionEnginePool6;
        _predictionEnginePool7 = predictionEnginePool7;
    }

    [HttpPost("accesoBancaMovil")]
    public async Task<ActionResult<MLModelAccesosBancaMovil.ModelOutput>> PredictAccesoBancaMovil([FromBody] MLModelAccesosBancaMovil.ModelInput input)
    {
        var prediction = await Task.FromResult(_predictionEnginePool1.Predict(input));
        return Ok(prediction);
    }

   [HttpPost("consumoTarjetasDebito")]
    public async Task<ActionResult<MLModelTarjetasDebito.ModelOutput>> PredictConsumoTarjetasDebito([FromBody] MLModelTarjetasDebito.ModelInput input)
    {

        Console.WriteLine(input.Hora);
        Console.WriteLine(input.Fecha);

        // Get the prediction
        var prediction = await Task.FromResult(_predictionEnginePool2.Predict(input));

        // Return the prediction result
        return Ok(prediction);
    } 


    [HttpPost("procesadorBff")]
    public async Task<ActionResult<MLModel1BFF.ModelOutput>> PredictBFF([FromBody] MLModel1BFF.ModelInput input)
    {
        var prediction = await Task.FromResult(_predictionEnginePool3.Predict(input));
        return Ok(prediction);
    }

    [HttpPost("micro")]
    public async Task<ActionResult<MLModel1BFF.ModelOutput>> PredictMicro([FromBody] MLModelMirco2.ModelInput input)
    {
        var prediction = await Task.FromResult(_predictionEnginePool4.Predict(input));
        return Ok(prediction);
    }

    [HttpPost("accesosProdunet")]
    public async Task<ActionResult<MLModelAccesos_Produnet.ModelOutput>> PredictAccesosProdunet ([FromBody] MLModelAccesos_Produnet.ModelInput input)
    {
        var prediction = await Task.FromResult(_predictionEnginePool5.Predict(input));
        return Ok(prediction);
    }

    [HttpPost("memoriaBff")]
    public async Task<ActionResult<MLModel_MemoriaBFF.ModelOutput>> PredictMemoriaBFF([FromBody] MLModel_MemoriaBFF.ModelInput input)
    {
        var prediction = await Task.FromResult(_predictionEnginePool6.Predict(input));
        return Ok(prediction);
    }

    [HttpPost("memoriaMicros")]
    public async Task<ActionResult<MLModelMemoria_Micros.ModelOutput>> PredictMemoriaMicros([FromBody] MLModelMemoria_Micros.ModelInput input)
    {
        var prediction = await Task.FromResult(_predictionEnginePool7.Predict(input));
        return Ok(prediction);
    }

    [HttpPost("predictAll")]
    public async Task<ActionResult<BffandMicroDTO>> PredictAll([FromBody] CommonInputDTO input)
    {
        var bffPrediction = await Task.FromResult(_predictionEnginePool3.Predict(new MLModel1BFF.ModelInput
        {
            Fecha = input.Fecha,
            Hora = input.Hora
        }));

        var microPrediction = await Task.FromResult(_predictionEnginePool4.Predict(new MLModelMirco2.ModelInput
        {
            Fecha = input.Fecha,
            Hora = input.Hora
        }));

        // Supongamos que tienes otros dos modelos llamados "Otro" y "Mas".
        var bffMemoryScore = await Task.FromResult(_predictionEnginePool6.Predict(new MLModel_MemoriaBFF.ModelInput
        {
            Fecha = input.Fecha,
            Hora = input.Hora
        }));

        var microMemoryscore = await Task.FromResult(_predictionEnginePool7.Predict(new MLModelMemoria_Micros.ModelInput
        {
            Date = input.Fecha,
            Time = input.Hora
        }));

        var combinedOutput = new BffandMicroDTO
        {
            BFFProcessorScore = bffPrediction.Score,
            MicroProcessorScore = microPrediction.Score,
            BFFMemoryScore = bffMemoryScore.Score,
            MicroMemoryScore = microMemoryscore.Score
        };

        return Ok(combinedOutput);
    }

}
