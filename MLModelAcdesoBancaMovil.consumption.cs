﻿// This file was auto-generated by ML.NET Model Builder.
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
namespace DashboardModels
{
    public partial class MLModelAcdesoBancaMovil
    {
        /// <summary>
        /// model input class for MLModelAcdesoBancaMovil.
        /// </summary>
        #region model input class
        public class ModelInput
        {
            [LoadColumn(0)]
            [ColumnName(@"Fecha")]
            public DateTime Fecha { get; set; }

            [LoadColumn(1)]
            [ColumnName(@"Hora")]
            public DateTime Hora { get; set; }

            [LoadColumn(2)]
            [ColumnName(@"Numero")]
            public float Numero { get; set; }

        }

        #endregion

        /// <summary>
        /// model output class for MLModelAcdesoBancaMovil.
        /// </summary>
        #region model output class
        public class ModelOutput
        {
            // Renombrado para evitar confusión con ModelInput
            [ColumnName(@"PredictedFecha")]
            public float PredictedFecha { get; set; }

            [ColumnName(@"PredictedHora")]
            public float PredictedHora { get; set; }

            [ColumnName(@"PredictedNumero")]
            public float PredictedNumero { get; set; }

            [ColumnName(@"Features")]
            public float[] Features { get; set; }

            [ColumnName(@"Score")]
            public float Score { get; set; }
        }
        #endregion

        private static string MLNetModelPath = Path.GetFullPath("MLModelAcdesoBancaMovil.mlnet");

        public static readonly Lazy<PredictionEngine<ModelInput, ModelOutput>> PredictEngine = new Lazy<PredictionEngine<ModelInput, ModelOutput>>(() => CreatePredictEngine(), true);


        private static PredictionEngine<ModelInput, ModelOutput> CreatePredictEngine()
        {
            var mlContext = new MLContext();
            ITransformer mlModel = mlContext.Model.Load(MLNetModelPath, out var _);
            return mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
        }

        /// <summary>
        /// Use this method to predict on <see cref="ModelInput"/>.
        /// </summary>
        /// <param name="input">model input.</param>
        /// <returns><seealso cref=" ModelOutput"/></returns>
        public static ModelOutput Predict(ModelInput input)
        {
            var predEngine = PredictEngine.Value;
            return predEngine.Predict(input);
        }
    }
}
