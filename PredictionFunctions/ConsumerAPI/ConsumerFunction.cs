using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ConsumerAPI.ServiceBusQueue;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace ConsumerAPI
{
    public static class Function1
    {
        [FunctionName("ConsumerAPI")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            SendQueue queue = new SendQueue();

            string runQueue = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "runQueue", true) == 0)
                .Value;

            if (runQueue == null)
            {
                // Get request body
                dynamic data = await req.Content.ReadAsAsync<object>();
                runQueue = data?.name;
            }

            HttpClient httpClient = new HttpClient();
            //httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", "ewogICJBbWJldklkIiA6IDQwMCwKICAiRGV2aWNlSWQiIDogInByb2dyYW1tZXJzIiwKICAiVG9rZW4iIDogIkY3N0FCRDJDLUU0RkQtNDQxNS1CRTkzLTBGNkY5NzNBNDUxRCIKfQ==");

            HttpResponseMessage response = await httpClient.GetAsync("http://stage-ipdv.adttemp.com.br/ipdvapi/api/Images/GetAll/");

            string responseBody = await response.Content.ReadAsStringAsync();

            await queue.SendMessagesAsync(responseBody);

            return runQueue == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Status: " + "BadRequest", "application/json")
                : req.CreateResponse(HttpStatusCode.OK, "Status: " + "Sucess", "application/json");
        }
    }
}
