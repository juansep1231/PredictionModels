using DashboardModels;
using MemoriaMicro;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;

using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class PredictionController : ControllerBase
{
    private readonly PredictionEnginePool<MLModelAcdesoBancaMovil.ModelInput, MLModelAcdesoBancaMovil.ModelOutput> _predictionEnginePool1; //J
    private readonly PredictionEnginePool<MLModelConsumoTarjetasDebito.ModelInput, MLModelConsumoTarjetasDebito.ModelOutput> _predictionEnginePool2; //J
    private readonly PredictionEnginePool<MLModel1BFF.ModelInput, MLModel1BFF.ModelOutput> _predictionEnginePool3; //F
    private readonly PredictionEnginePool<MLModelMirco2.ModelInput, MLModelMirco2.ModelOutput> _predictionEnginePool4; //F
    private readonly PredictionEnginePool<MLModelAccesos_Produnet.ModelInput, MLModelAccesos_Produnet.ModelOutput> _predictionEnginePool5; //K
    private readonly PredictionEnginePool<MLModel_MemoriaBFF.ModelInput, MLModel_MemoriaBFF.ModelOutput> _predictionEnginePool6; //D
    private readonly PredictionEnginePool<MLModelMemoria_Micros.ModelInput, MLModelMemoria_Micros.ModelOutput> _predictionEnginePool7; //K
    public PredictionController(
        PredictionEnginePool<MLModelAcdesoBancaMovil.ModelInput, MLModelAcdesoBancaMovil.ModelOutput> predictionEnginePool1,
        PredictionEnginePool<MLModelConsumoTarjetasDebito.ModelInput, MLModelConsumoTarjetasDebito.ModelOutput> predictionEnginePool2,
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
    public async Task<ActionResult<MLModelAcdesoBancaMovil.ModelOutput>> PredictAccesoBancaMovil([FromBody] MLModelAcdesoBancaMovil.ModelInput input)
    {
        var prediction = await Task.FromResult(_predictionEnginePool1.Predict(input));
        return Ok(prediction);
    }

    [HttpPost("consumoTarjetasDebito")]
    public async Task<ActionResult<MLModelConsumoTarjetasDebito.ModelOutput>> PredictConsumoTarjetasDebito([FromBody] MLModelConsumoTarjetasDebito.ModelInput input)
    {
        var prediction = await Task.FromResult(_predictionEnginePool2.Predict(input));
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

}
