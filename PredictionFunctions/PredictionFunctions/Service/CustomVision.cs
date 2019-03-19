using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredictionFunctions.Service
{
    public class CustomVision
    {
        const string southcentralus = "https://southcentralus.api.cognitive.microsoft.com";

        static string predictionKey = "a58f3ca5856c491db0b73b87cb1118cf";

        int count = 0;
        CustomVisionPredictionClient endpoint = new CustomVisionPredictionClient();

        public CustomVisionPredictionClient InitializeServe()
        {
            CustomVisionPredictionClient ed = new CustomVisionPredictionClient()
            {
                ApiKey = predictionKey,
                Endpoint = southcentralus
            };

            return ed;
        }

        public CustomVision()
        {
            endpoint = InitializeServe();
        }


        public async Task<List<PredictionModel>> PredictionImages(string url)
        {
            ImageUrl imgUrl = new ImageUrl();
            imgUrl.Url = url;
            

            var predictions = new List<PredictionModel>();

            var resultImageUrl = await endpoint.PredictImageUrlAsync(Guid.Parse("cbfa66a3-9815-47d6-a389-7438e468ac15"), imgUrl);

            foreach (var item in resultImageUrl.Predictions.OrderBy(x => x.Probability))
            {
                var resultPredction = new PredictionModel(Math.Round(item.Probability * 100), item.TagId, item.TagName, item.BoundingBox);

                if (resultPredction.Probability > 90)
                {
                    resultPredction.BoundingBox.Top = Convert.ToInt32(resultPredction.BoundingBox.Top * 380);
                    resultPredction.BoundingBox.Height = Convert.ToInt32(resultPredction.BoundingBox.Height * 380);
                    resultPredction.BoundingBox.Left = Convert.ToInt32(resultPredction.BoundingBox.Left * 700);
                    resultPredction.BoundingBox.Width = Convert.ToInt32(resultPredction.BoundingBox.Width * 700);
                    predictions.Add(resultPredction);
                    count++;
                }

            }

            if (count == 0)
            {
                var plowForecast = resultImageUrl.Predictions.OrderBy(x => x.Probability).ToList();

                var resultPredction = new PredictionModel(Convert.ToInt32(plowForecast[plowForecast.Count - 1].Probability * 100), plowForecast[plowForecast.Count - 1].TagId, plowForecast[plowForecast.Count - 1].TagName, plowForecast[plowForecast.Count - 1].BoundingBox);
                predictions.Add(resultPredction);
            }

            return predictions;
        }

    }
}

