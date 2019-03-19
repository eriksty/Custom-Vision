using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace FunctionApp1
{
    
    public static class Function1
    {


       
        private static SqlConnection connection = new SqlConnection("Server=DESKTOP-2ME9TD4\\SQLEXPRESS;Database=Predictions;Trusted_Connection=True");
        [FunctionName("Function1")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {

            //try
            //{
            //    await connection.OpenAsync();
            //}
            //catch (Exception)
            //{

            //    throw;
            //}

            Prediction pd = new Prediction();

            string url = "https://informebairro.com.br/wp-content/uploads/2018/01/aula-de-instrumentos-musicais-violao-guitarra.jpg";
            for (int i = 0; i < 2; i++)
            {
                await SendQueue.SendMessagesAsync(url);
                await sendQueue1();
                await sendQueue2();
            }

            log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            string name = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
                .Value;

            if (name == null)
            {
                // Get request body
                dynamic data = await req.Content.ReadAsAsync<object>();
                name = data?.name;
            }
            
            const string southcentralus = "https://southcentralus.api.cognitive.microsoft.com";

            string predictionKey = "a58f3ca5856c491db0b73b87cb1118cf";

            CustomVisionPredictionClient endpoint = new CustomVisionPredictionClient()
            {
                ApiKey = predictionKey,
                Endpoint = southcentralus
            };

            ImageUrl imgUrl = new ImageUrl();
            imgUrl.Url = "https://http2.mlstatic.com/guitarra-tagima-pr-200-special-pr200-sunburst-D_NQ_NP_894387-MLB26271081482_112017-F.jpg";

            var c = new List<PredictionModel>();

            var resultImageUrl = endpoint.PredictImageUrl(Guid.Parse("cbfa66a3-9815-47d6-a389-7438e468ac15"), imgUrl);


            foreach (var item in resultImageUrl.Predictions.OrderBy(x => x.Probability))
            {
                var pm = new PredictionModel(Math.Round(item.Probability * 100), item.TagId, item.TagName, item.BoundingBox);

                if (pm.Probability > 59)
                {
                    pm.BoundingBox.Top = Convert.ToInt32(pm.BoundingBox.Top * 380);
                    pm.BoundingBox.Height = Convert.ToInt32(pm.BoundingBox.Height * 380);
                    pm.BoundingBox.Left = Convert.ToInt32(pm.BoundingBox.Left * 700);
                    pm.BoundingBox.Width = Convert.ToInt32(pm.BoundingBox.Width * 700);
                    c.Add(pm);
                }

            }
            
            foreach (var item in c)
            {
                pd.Id = Guid.NewGuid();
                pd.Url = url;
                pd.CanAdvertisement = true;
                pd.Probability = item.Probability;

                string sql = "INSERT INTO Predictions(Id,Url,CanAdvertisement,Probability) VALUES(@param1,@param2,@param3,@param3)";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@param1", SqlDbType.UniqueIdentifier).Value = pd.Id;
                    cmd.Parameters.Add("@param2", SqlDbType.VarChar, 100).Value = pd.Url;
                    cmd.Parameters.Add("@param3", SqlDbType.Bit).Value = pd.CanAdvertisement;
                    cmd.Parameters.Add("@param4", SqlDbType.Float).Value = pd.Probability;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
            }


            var jsonToReturn = JsonConvert.SerializeObject(resultImageUrl);

            return req.CreateResponse(HttpStatusCode.OK, jsonToReturn, "application/json");

        }


        public static async Task sendQueue1()
        {
            var url = "https://http2.mlstatic.com/guitarra-tagima-pr-200-special-pr200-sunburst-D_NQ_NP_894387-MLB26271081482_112017-F.jpg";
            await SendQueue.SendMessagesAsync(url);
        }

        public static async Task sendQueue2()
        {
            var url = "https://http2.mlstatic.com/guitarra-tagima-pr-200-special-pr200-sunburst-D_NQ_NP_894387-MLB26271081482_112017-F.jpg";
            await SendQueue.SendMessagesAsync(url);
        }
    }

}
