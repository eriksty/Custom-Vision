using ExampleServiceBus.Model;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleServiceBus.Service
{
    public class GetQueue
    {
        const string ServiceBusConnectionString = "Endpoint=sb://azure-semple-queue.servicebus.windows.net/;SharedAccessKeyName=bug-queue-acess;SharedAccessKey=nYyPNKtpRcv9KPmC30qSyhl5bZO5vfkxEKCVForsrbs=;";
        const string QueueName = "bug-queue";
        static IQueueClient queueClient;

        public static async Task RegisterOnMessageHandlerAndReceiveMessages()
        {
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName);

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,

                AutoComplete = false
            };

           queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        private static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {

            var context = new DataContext();
            var obj = new Teste();

            var msgBody = Encoding.UTF8.GetString(message.Body);

            var jsonQueue = JsonConvert.DeserializeObject<Teste>(msgBody);
            jsonQueue.Id = Guid.NewGuid();

            context.Teste.Add(jsonQueue);
            context.SaveChanges();
            await queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}
