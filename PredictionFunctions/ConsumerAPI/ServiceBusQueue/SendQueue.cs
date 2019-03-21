using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerAPI.ServiceBusQueue
{
    public class SendQueue
    {
        const string ServiceBusConnectionString = "Endpoint=sb://azure-semple-queue.servicebus.windows.net/;SharedAccessKeyName=bug-queue-acess;SharedAccessKey=nYyPNKtpRcv9KPmC30qSyhl5bZO5vfkxEKCVForsrbs=;";
        const string QueueName = "bug-queue";
        static IQueueClient queueClient;

        public SendQueue()
        {
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName);
        }

        public async Task SendMessagesAsync(string data)
        {
            try
            {
                queueClient = new QueueClient(ServiceBusConnectionString, QueueName);

                var item = JsonConvert.DeserializeObject(data);
                var message = new Message(Encoding.UTF8.GetBytes(data));

                await queueClient.SendAsync(message);

                }
            catch (Exception exception)
            {
                await queueClient.CloseAsync();
                throw new Exception(exception.Message);
            }
        }

        public async Task Finish()
        {
            await queueClient.CloseAsync();
        }
    }
}
