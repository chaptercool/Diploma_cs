using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Maui.Storage;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace Diploma_cs
{
    public partial class MainComputingModule
    {
        public class ModelInput
        {
            public float AvgConsumedWeek { get; set; }
            public float AvgBoughtPacks { get; set; }
            public float BoughtSum { get; set; }
            public float ReduceBy { get; set; }
        }

        public class ModelOutput
        {
            public float AvgConsumedWeek { get; set; }
            public float AvgBoughtPacks { get; set; }
            public float BoughtSum { get; set; }
            public float ReduceBy { get; set; }
            public float[] Features { get; set; } = Array.Empty<float>();
            public float Score { get; set; }
        }

        private const string AppPackageOnnxModelPath = "MachineLearning/MainComputingModule.onnx";

        private static readonly Lazy<InferenceSession> Session = new(() =>
        {
            using var stream = FileSystem.OpenAppPackageFileAsync(AppPackageOnnxModelPath).GetAwaiter().GetResult();
            using var ms = new MemoryStream();
            stream.CopyTo(ms);

            var options = new SessionOptions();
            var session = new InferenceSession(ms.ToArray(), options);

            Debug.WriteLine($"ONNX Inputs: {string.Join(", ", session.InputMetadata.Keys)}");
            Debug.WriteLine($"ONNX Outputs: {string.Join(", ", session.OutputMetadata.Keys)}");

            foreach (var kvp in session.InputMetadata)
            {
                var dims = string.Join(",", kvp.Value.Dimensions.Select(d => d.ToString()));
                Debug.WriteLine($"ONNX Input '{kvp.Key}' dims: [{dims}] type: {kvp.Value.ElementType}");
            }

            return session;
        }, true);

        public static ModelOutput Predict(ModelInput input)
        {
            var session = Session.Value;

            const string avgConsumedName = "AvgConsumedWeek";
            const string avgBoughtName = "AvgBoughtPacks";
            const string boughtSumName = "BoughtSum";
            const string reduceByName = "ReduceBy";

            const string scoreOutputName = "Score.output";

            if (!session.InputMetadata.ContainsKey(avgConsumedName)
                || !session.InputMetadata.ContainsKey(avgBoughtName)
                || !session.InputMetadata.ContainsKey(boughtSumName)
                || !session.InputMetadata.ContainsKey(reduceByName))
            {
                throw new InvalidOperationException(
                    $"ONNX model inputs do not match expected names. Available: [{string.Join(", ", session.InputMetadata.Keys)}]");
            }

            if (!session.OutputMetadata.ContainsKey(scoreOutputName))
            {
                throw new InvalidOperationException(
                    $"ONNX model output '{scoreOutputName}' not found. Available: [{string.Join(", ", session.OutputMetadata.Keys)}]");
            }

            // Each input expects shape [-1, 1] => use [1, 1] for a single row.
            var tAvgConsumed = new DenseTensor<float>(new[] { 1, 1 });
            tAvgConsumed[0, 0] = input.AvgConsumedWeek;

            var tAvgBought = new DenseTensor<float>(new[] { 1, 1 });
            tAvgBought[0, 0] = input.AvgBoughtPacks;

            var tBoughtSum = new DenseTensor<float>(new[] { 1, 1 });
            tBoughtSum[0, 0] = input.BoughtSum;

            // Your DebugPage doesn't provide ReduceBy. The exported graph still requires it as an input.
            // Use the value from ModelInput (defaults to 0f if not set).
            var tReduceBy = new DenseTensor<float>(new[] { 1, 1 });
            tReduceBy[0, 0] = input.ReduceBy;

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor(avgConsumedName, tAvgConsumed),
                NamedOnnxValue.CreateFromTensor(avgBoughtName, tAvgBought),
                NamedOnnxValue.CreateFromTensor(boughtSumName, tBoughtSum),
                NamedOnnxValue.CreateFromTensor(reduceByName, tReduceBy),
            };

            using var results = session.Run(inputs);

            var score = results.First(r => r.Name == scoreOutputName)
                               .AsTensor<float>()
                               .ToArray()
                               .FirstOrDefault();

            return new ModelOutput
            {
                AvgConsumedWeek = input.AvgConsumedWeek,
                AvgBoughtPacks = input.AvgBoughtPacks,
                BoughtSum = input.BoughtSum,
                ReduceBy = input.ReduceBy,
                Features = new[] { input.AvgConsumedWeek, input.AvgBoughtPacks, input.BoughtSum },
                Score = score
            };
        }
    }
}