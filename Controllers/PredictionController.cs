using DashboardModels;
using DashboardModels.Controllers;
using DashboardModels.Controllers.DTOs;
using DashboardModels.Utils;
using MemoriaMicro;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;
using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;


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
    private readonly PredictionEnginePool<Servidores.ModelInput, Servidores.ModelOutput> _predictionEnginePool8; //K
    private readonly PredictionEnginePool<MLModel_DebitoActualizado.ModelInput, MLModel_DebitoActualizado.ModelOutput> _predictionEnginePool9; //J
    private readonly IHttpClientFactory _httpClientFactory;

    public PredictionController(
        PredictionEnginePool<MLModelAccesosBancaMovil.ModelInput, MLModelAccesosBancaMovil.ModelOutput> predictionEnginePool1,
        PredictionEnginePool<MLModelTarjetasDebito.ModelInput, MLModelTarjetasDebito.ModelOutput> predictionEnginePool2,
        PredictionEnginePool<MLModel1BFF.ModelInput, MLModel1BFF.ModelOutput> predictionEnginePool3,
        PredictionEnginePool<MLModelMirco2.ModelInput, MLModelMirco2.ModelOutput> predictionEnginePool4,
        PredictionEnginePool<MLModelAccesos_Produnet.ModelInput, MLModelAccesos_Produnet.ModelOutput> predictionEnginePool5,
        PredictionEnginePool<MLModel_MemoriaBFF.ModelInput, MLModel_MemoriaBFF.ModelOutput> predictionEnginePool6,
        PredictionEnginePool<MLModelMemoria_Micros.ModelInput, MLModelMemoria_Micros.ModelOutput> predictionEnginePool7,
        PredictionEnginePool<Servidores.ModelInput, Servidores.ModelOutput> predictionEnginePool8,
        PredictionEnginePool<MLModel_DebitoActualizado.ModelInput, MLModel_DebitoActualizado.ModelOutput> predictionEnginePool9,

       IHttpClientFactory httpClientFactory
        )
    {
        _predictionEnginePool1 = predictionEnginePool1;
        _predictionEnginePool2 = predictionEnginePool2;
        _predictionEnginePool3 = predictionEnginePool3;
        _predictionEnginePool4 = predictionEnginePool4;
        _predictionEnginePool5 = predictionEnginePool5;
        _predictionEnginePool6 = predictionEnginePool6;
        _predictionEnginePool7 = predictionEnginePool7;
        _predictionEnginePool8 = predictionEnginePool8;
       _predictionEnginePool9 = predictionEnginePool9;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("accesoBancaMovil")]
    public async Task<ActionResult<MLModelAccesosBancaMovil.ModelOutput>> PredictAccesoBancaMovil([FromBody] MLModelAccesosBancaMovil.ModelInput input)
    {

        DateTime parsedDate = DateTime.ParseExact(input.Fecha, "MM/dd/yyyy", CultureInfo.InvariantCulture);


        string formattedDate = parsedDate.ToString("dd/MM/yyyy");


        input.Fecha = formattedDate;
        var prediction = await Task.FromResult(_predictionEnginePool1.Predict(input));
        return Ok(prediction);
    }

    [HttpPost("consumoTarjetasDebito")]
    public async Task<ActionResult<MLModelTarjetasDebito.ModelOutput>> PredictConsumoTarjetasDebito([FromBody] MLModelTarjetasDebito.ModelInput input)
    {

        // Get the prediction
        var prediction = await Task.FromResult(_predictionEnginePool2.Predict(input));

        // Return the prediction result
        return Ok(prediction);
    }

    [HttpPost("consumoTarjetasDebitoActual")]
    public async Task<ActionResult<MLModelTarjetasDebito.ModelOutput>> PredictConsumoTarjetasDebitoActual([FromBody] MLModel_DebitoActualizado.ModelInput input)
    {

        // Get the prediction
        var prediction = await Task.FromResult(_predictionEnginePool9.Predict(input));

        // Return the prediction result
        return Ok(prediction);
    }

    //Para los servidores diariamente
    [HttpPost("predictMaxHourly")]
    public async Task<ActionResult<BffandMicroDTO>> PredictMaxHourly([FromBody] PicoMaximoDTO input)
    {
        // Inicializar variables para almacenar los máximos y las horas correspondientes
        string maxBFFProcessorHour = string.Empty;
        float maxBFFProcessorScore = float.MinValue;

        string maxMicroProcessorHour = string.Empty;
        float maxMicroProcessorScore = float.MinValue;

        string maxBFFMemoryHour = string.Empty;
        float maxBFFMemoryScore = float.MinValue;

        string maxMicroMemoryHour = string.Empty;
        float maxMicroMemoryScore = float.MinValue;

        // Iterar sobre todas las horas del día
        foreach (var hour in ListaConstantes.listaHoras)
        {
            // Realizar predicción para BFFProcessor
            var bffPrediction = await Task.FromResult(_predictionEnginePool3.Predict(new MLModel1BFF.ModelInput
            {
                Fecha = input.Fecha,
                Hora = hour.horaCompleta
            }));

            // Realizar predicción para MicroProcessor
            var microPrediction = await Task.FromResult(_predictionEnginePool4.Predict(new MLModelMirco2.ModelInput
            {
                Fecha = input.Fecha,
                Hora = hour.horaCompleta
            }));

            // Realizar predicción para BFFMemory
            var bffMemoryScore = await Task.FromResult(_predictionEnginePool6.Predict(new MLModel_MemoriaBFF.ModelInput
            {
                Fecha = input.Fecha,
                Hora = hour.horaCompleta
            }));

            // Realizar predicción para MicroMemory
            var microMemoryscore = await Task.FromResult(_predictionEnginePool7.Predict(new MLModelMemoria_Micros.ModelInput
            {
                Date = input.Fecha,
                Time = hour.horaCompleta
            }));

            // Actualizar máximos y las horas correspondientes
            if (bffPrediction.Score > maxBFFProcessorScore)
            {
                maxBFFProcessorScore = bffPrediction.Score;
                maxBFFProcessorHour = hour.horaCompleta; // Guardar la hora del máximo
            }

            if (microPrediction.Score > maxMicroProcessorScore)
            {
                maxMicroProcessorScore = microPrediction.Score;
                maxMicroProcessorHour = hour.horaCompleta; // Guardar la hora del máximo
            }

            if (bffMemoryScore.Score > maxBFFMemoryScore)
            {
                maxBFFMemoryScore = bffMemoryScore.Score;
                maxBFFMemoryHour = hour.horaCompleta; // Guardar la hora del máximo
            }

            if (microMemoryscore.Score > maxMicroMemoryScore)
            {
                maxMicroMemoryScore = microMemoryscore.Score;
                maxMicroMemoryHour = hour.horaCompleta; // Guardar la hora del máximo
            }
        }

        // Crear el DTO con los valores máximos y las horas correspondientes
        var combinedOutput = new BffandMicroMaxOuputDTO
        {
            BFFProcessorScores = new Dictionary<string, float> { { maxBFFProcessorHour, maxBFFProcessorScore } },
            MicroProcessorScores = new Dictionary<string, float> { { maxMicroProcessorHour, maxMicroProcessorScore } },
            BFFMemoryScores = new Dictionary<string, float> { { maxBFFMemoryHour, maxBFFMemoryScore } },
            MicroMemoryScores = new Dictionary<string, float> { { maxMicroMemoryHour, maxMicroMemoryScore } }
        };

        // Devolver el resultado con los scores máximos y las horas correspondientes
        return Ok(combinedOutput);
    }

    //Para los servidores por hora 
    [HttpPost("predictAll")]
    public async Task<ActionResult<BffandMicroDTO>> PredictAll([FromBody] CommonInputDTO input)
    {
        Console.WriteLine("Fecha: " + input.Fecha + "Hora" + input.Hora);
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

        Console.WriteLine(combinedOutput.BFFMemoryScore + " " + combinedOutput.MicroProcessorScore + " " + combinedOutput.BFFMemoryScore);

        return Ok(combinedOutput);
    }


    [HttpPost("top10PredictionSum")]
    public async Task<ActionResult<IEnumerable<Top10OutputDTO>>> GetTop10PredictionSumForRemainingYear([FromBody] Top10InputDTO input)
    {
        // Inicializar variables
        List<Top10OutputDTO> predictionResults = new List<Top10OutputDTO>();

        // Realizar predicciones y sumar las predicciones de ambos modelos
        foreach (var data in ListaConstantes.listaHoras)
        {
            var prediction1 = _predictionEnginePool1.Predict(new MLModelAccesosBancaMovil.ModelInput
            {
                Fecha = input.Fecha,
                Hora = data.horaCompleta
            });
            var prediction2 = _predictionEnginePool5.Predict(new MLModelAccesos_Produnet.ModelInput
            {
                Date = input.Fecha,
                Time = data.horaCompleta
            });
            float sumPrediction = prediction1.Score + prediction2.Score;


            // Agregar los resultados a la lista
            predictionResults.Add(new Top10OutputDTO
            {
                Hour = data.horaCompleta,
                SumPrediction = sumPrediction
            });

        }

        return Ok(predictionResults);
    }



    [HttpPost("top10PredictionSumDebitCard")]
    public async Task<ActionResult<IEnumerable<Top10OutputDTO>>> GetTop10PredictionSumDebitCard([FromBody] Top10InputDTO input)
    {
        // Inicializar variables
        List<Top10OutputDTO> predictionResults = new List<Top10OutputDTO>();

        // Realizar predicciones y sumar las predicciones de ambos modelos
        foreach (var data in ListaConstantes.listaHoras)
        {
            var prediction = _predictionEnginePool2.Predict(new MLModelTarjetasDebito.ModelInput
            {
                Fecha = input.Fecha,
                Hora = data.horaCompleta
            });

            // Agregar los resultados a la lista
            predictionResults.Add(new Top10OutputDTO
            {
                Hour = data.horaCompleta,
                SumPrediction = prediction.Score
            });

        }

        return Ok(predictionResults);
    }

    [HttpPost("top10PredictionSumDebitCardActual")]
    public async Task<ActionResult<IEnumerable<Top10OutputDTO>>> GetTop10PredictionSumDebitCardActual([FromBody] Top10InputDTO input)
    {
        // Inicializar variables
        List<Top10OutputDTO> predictionResults = new List<Top10OutputDTO>();

        // Realizar predicciones y sumar las predicciones de ambos modelos
        foreach (var data in ListaConstantes.listaHoras)
        {
            var prediction = _predictionEnginePool9.Predict(new MLModel_DebitoActualizado.ModelInput
            {
                Fecha = input.Fecha,
                Hora = data.horaCompleta
            });

            // Agregar los resultados a la lista
            predictionResults.Add(new Top10OutputDTO
            {
                Hour = data.horaCompleta,
                SumPrediction = prediction.Score
            });

        }

        return Ok(predictionResults);
    }

    //top 10 produnet y movil 
    [HttpGet("yearlySum")]
    public async Task<ActionResult<IEnumerable<PredictionSumResult>>> GetTop10PredictionSum()
    {
        List<PredictionSumResult> dailySums = new List<PredictionSumResult>();
        var _httpClient = _httpClientFactory.CreateClient();

        DateTime currentDate = DateTime.Today;
        DateTime endOfYear = new DateTime(currentDate.Year, 12, 31); // Último día del año

        while (currentDate <= endOfYear)
        {
            string formattedDateMovil = currentDate.ToString("dd/MM/yyyy");
            string formattedDateProdunet = currentDate.ToString("yyyy-MM-ddT00:00:00.000Z");

            float dailySumPrediction = 0;

            foreach (var hour in ListaConstantes.listaHoras)
            {
                // Predicción de "movil" usando el modelo local
                var prediction1 = _predictionEnginePool1.Predict(new MLModelAccesosBancaMovil.ModelInput
                {
                    Fecha = formattedDateMovil,
                    Hora = hour.horaCompleta
                });

                // Predicción de "produnet" mediante una petición POST a una URL externa
                try
                {
                    // Crear el cuerpo de la solicitud
                    var requestBody = new
                    {
                        Inputs = new
                        {
                            data = new[]
                            {
                            new
                            {
                                FECHA = formattedDateProdunet,
                                HORA = hour.horaEntero
                            }
                        }
                        },
                        GlobalParameters = 1
                    };

                    // Serializar el cuerpo a JSON
                    var jsonRequestBody = JsonConvert.SerializeObject(requestBody);
                    Console.WriteLine(jsonRequestBody.ToString());
                    // Crear el contenido HTTP
                    var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

                    // Realizar la petición POST
                    var response = await _httpClient.PostAsync("http://e795f5d6-3414-4512-985f-c70bc1db2eb7.westus2.azurecontainer.io/score", content);

                    if (!response.IsSuccessStatusCode)
                    {
                        return StatusCode((int)response.StatusCode,"Error al obtener la predicción de produnet");
                    }

                    // Leer el contenido de la respuesta
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Deserializar la respuesta para obtener la predicción
                    var prediction2Response = JsonConvert.DeserializeObject<ProdunetPredictionResponseDTO>(responseContent);

                    // Validar que la respuesta contenga datos
                    if (prediction2Response.Results == null || prediction2Response.Results.Count == 0)
                    {
                        return StatusCode(500, "La respuesta de produnet no contiene datos válidos.");
                    }

                    // Obtener el score de la predicción
                    float produnetScore = prediction2Response.Results[0];

                    // Sumar las predicciones
                    float sumPrediction = prediction1.Score + produnetScore;

                    dailySumPrediction += sumPrediction;
                }
                catch (Exception ex)
                {
                    // Manejar la excepción
                    return StatusCode(500, $"Error al procesar la predicción de produnet: {ex.Message}");
                }
            }

            dailySums.Add(new PredictionSumResult
            {
                Date = formattedDateMovil,
                SumPrediction = dailySumPrediction
            });

            currentDate = currentDate.AddDays(1);
        }

        var top10Results = dailySums
            .OrderByDescending(x => x.SumPrediction)
            .Take(10)
            .ToList();

        // Retornar el top 10 de sumas diarias
        return Ok(top10Results);
    }

    

    [HttpGet("yearlySumTesting")]
    public async Task<ActionResult<IEnumerable<PredictionSumResult>>> GetTop10PredictionSumTesting()
    {

        List<PredictionSumResult> dailySums = new List<PredictionSumResult>();


        DateTime currentDate = DateTime.Today;
        DateTime endOfYear = new DateTime(currentDate.Year, 12, 31); // Último día del año

        while (currentDate <= endOfYear)
        {

            string formattedDate = currentDate.ToString("dd/MM/yyyy");


            float dailySumPrediction = 0;


            foreach (var hour in ListaConstantes.listaHoras)
            {

                var prediction1 = _predictionEnginePool1.Predict(new MLModelAccesosBancaMovil.ModelInput
                {
                    Fecha = formattedDate,
                    Hora = hour.horaCompleta
                });


                var prediction2 = _predictionEnginePool5.Predict(new MLModelAccesos_Produnet.ModelInput
                {
                    Date = formattedDate,
                    Time = hour.horaCompleta
                });


                float sumPrediction = prediction1.Score + prediction2.Score;


                dailySumPrediction += sumPrediction;
            }


            dailySums.Add(new PredictionSumResult
            {
                Date = formattedDate,

                SumPrediction = dailySumPrediction
            });



            currentDate = currentDate.AddDays(1);
        }


        var top10Results = dailySums
            .OrderByDescending(x => x.SumPrediction)
            .Take(10)
            .ToList();

        // Retornar el top 10 de sumas diarias
        return Ok(top10Results);
    }

    //top 10 produnet y movil 
    [HttpGet("yearlySumDebitCard")]
    public async Task<ActionResult<IEnumerable<PredictionSumResult>>> GetTop10MovilCard()
    {

        List<PredictionSumResult> dailySums = new List<PredictionSumResult>();


        DateTime currentDate = DateTime.Today;
        DateTime endOfYear = new DateTime(currentDate.Year, 12, 31); // Último día del año

        while (currentDate <= endOfYear)
        {

            string formattedDate = currentDate.ToString("dd/MM/yyyy");


            float dailySumPrediction = 0;


            foreach (var hour in ListaConstantes.listaHoras)
            {

                var prediction = _predictionEnginePool2.Predict(new MLModelTarjetasDebito.ModelInput
                {
                    Fecha = formattedDate,
                    Hora = hour.horaCompleta
                });


                dailySumPrediction += prediction.Score;
            }


            dailySums.Add(new PredictionSumResult
            {
                Date = formattedDate,
                SumPrediction = dailySumPrediction
            });

            currentDate = currentDate.AddDays(1);
        }


        var top10Results = dailySums
            .OrderByDescending(x => x.SumPrediction)
            .Take(10)
            .ToList();

        // Retornar el top 10 de sumas diarias
        return Ok(top10Results);
    }


    //top 10 produnet y movil 
    [HttpGet("yearlySumDebitCardActual")]
    public async Task<ActionResult<IEnumerable<PredictionSumResult>>> GetTop10MovilCardActual()
    {

        List<PredictionSumResult> dailySums = new List<PredictionSumResult>();


        DateTime currentDate = DateTime.Today;
        DateTime endOfYear = new DateTime(2025, 01, 31); // Último día del año

        while (currentDate <= endOfYear)
        {

            string formattedDate = currentDate.ToString("dd/MM/yyyy");


            float dailySumPrediction = 0;


            foreach (var hour in ListaConstantes.listaHoras)
            {

                var prediction = _predictionEnginePool2.Predict(new MLModelTarjetasDebito.ModelInput
                {
                    Fecha = formattedDate,
                    Hora = hour.horaCompleta
                });


                dailySumPrediction += prediction.Score;
            }


            dailySums.Add(new PredictionSumResult
            {
                Date = formattedDate,
                SumPrediction = dailySumPrediction
            });

            currentDate = currentDate.AddDays(1);
        }


        var top10Results = dailySums
            .OrderByDescending(x => x.SumPrediction)
            .ToList();

        // Retornar el top 10 de sumas diarias
        return Ok(top10Results);
    }

    [HttpPost("serverPredictions")]
    public async Task<ActionResult<IEnumerable<ServidoresDTO>>> GetAllServersPrediction([FromBody] CommonInputDTO input)
    {
        // Inicializar variables
        List<ServidoresDTO> predictionResults = new List<ServidoresDTO>();


        // Realizar predicciones y sumar las predicciones de ambos modelos
        foreach (var server in ListaConstantes.listaDatos)
        {
            var prediction = _predictionEnginePool8.Predict(new Servidores.ModelInput
            {
                Fecha = input.Fecha,
                Hora = input.Hora,
                Servidor = server
            });

            // Agregar los resultados a la lista
            predictionResults.Add(new ServidoresDTO
            {
                Name = server,
                Usage = prediction.Score.ToString()
            });


        }

        return Ok(predictionResults);
    }

    public class MaxPredictionResult
    {
        public float MaxPrediction { get; set; }
        public string Date { get; set; }
        public string Hour { get; set; }
    }


    // DTO para el resultado de la suma de predicciones
    public class PredictionSumResult
    {
        public string Date { get; set; }
        public float SumPrediction { get; set; }
    }
}
