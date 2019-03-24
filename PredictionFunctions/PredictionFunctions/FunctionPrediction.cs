using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using PredictionFunctions.Queries;
using PredictionFunctions.Service;

namespace PredictionFunctions
{
    public static class FunctionPrediction
    {
        [FunctionName("Prediction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            CustomVision ct = new CustomVision();
            PredictionQueries pq = new PredictionQueries();

            var url = "https://http2.mlstatic.com/D_NP_674831-MLB26409090054_112017-Q.jpg";

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

            var result = await ct.PredictionImages(url);

            await pq.Add(result, url);

            var jsonToReturn = JsonConvert.SerializeObject(name);

            return name == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Status: " + "BadRequest", "application/json")
                : req.CreateResponse(HttpStatusCode.OK, "Status: " + "Sucess", "application/json");
        }
    }
}
