using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RedisExample
{
    class Program
    {
        const string ServiceBusConnectionString = "Endpoint=sb://briostaging.servicebus.windows.net/;SharedAccessKeyName=TransactionsTopic;SharedAccessKey=KVj1aHEaFD0G6b8Sv1ezIe22u/ag0YS3SyR5H5jFLKU=";
        const string TopicName = "transactions";
        const string SubscriptionName = "transaction";
        static ISubscriptionClient subscriptionClient;

        static void Main(string[] args)
        {
            //using (var client = new RedisClient("redis-18081.c114.us-east-1-4.ec2.cloud.redislabs.com", 18081, "1qc036dKiiinGcYfUjvy7wBdO8pUjCzx"))
            //{
            //    if (args.Count() < 1)
            //    {
            //        //JSON Format -- Easier
            //        client.SetEntryInHash("es-MX", "Reg-NotFound", "Error: no encontrado");
            //        client.SetEntryInHash("es-MX", "Reg-Duplicate", "Error: este registro ya existe");

            //        ////Multiple Values JSON Format
            //        //client.SetEntryInHash("es-MX", "Reg", "{\"Reg-NotFound\":\"Error: no encontrado\"},{\"Reg-Duplicate\":\"Error: este registro ya existe\"}");
            //        Console.WriteLine("Utiliza Argumentos. Ejemplo: 'dotnet run {ErrorCode}'");
            //        Console.WriteLine("---Presiona una tecla para salir---");
            //        Console.ReadKey();
            //    }
            //    else
            //    {
            //        ////Hash with multiple values
            //        //var data = client.GetValueFromHash("es-US",args[0]);
            //        //var deserializeData = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            //        //var list = deserializeData.Where(p => p.Key.Contains(args[0]));
            //        //List<ErrorModel> lista = list.Select(p => new ErrorModel { Code = p.Key, Message = p.Value }).ToList();
            //        //lista.ForEach(error => Console.WriteLine($"{error.Code} => {error.Message}"));
            //        //Console.ReadKey();


            //        //Hash JSON format
            //        var data = client.GetAllEntriesFromHash("es-MX");
            //        var list = data.Where(p => p.Key.Contains(args[0]));
            //        List<ErrorModel> lista = list.Select(p => new ErrorModel { Code = p.Key, Message = p.Value }).ToList();
            //        lista.ForEach(error => Console.WriteLine($"{error.Code} => {error.Message}"));

            //        Console.WriteLine("---Presiona una tecla para salir---");
            //        Console.ReadKey();
            //    }
            //}
            MainAsync().GetAwaiter().GetResult();
        }
        static async Task MainAsync()
        {
            subscriptionClient = new SubscriptionClient(ServiceBusConnectionString, TopicName, SubscriptionName, ReceiveMode.PeekLock);

            Console.WriteLine("======================================================");
            Console.WriteLine("Press ENTER key to exit after receiving all the messages.");
            Console.WriteLine("======================================================");

            // Register subscription message handler and receive messages in a loop.
            RegisterOnMessageHandlerAndReceiveMessages();

            Console.ReadKey();

            await subscriptionClient.CloseAsync();
        }

        static void RegisterOnMessageHandlerAndReceiveMessages()
        {
            // Configure the message handler options in terms of exception handling, number of concurrent messages to deliver, etc.
            var messageHandlerOptions = new SessionHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentSessions = 100,
                AutoComplete = true
            };

            // Register the function that processes messages.
            subscriptionClient.RegisterSessionHandler(ProcessMessagesAsync, messageHandlerOptions);
            Console.ReadKey();
        }

        static async Task ProcessMessagesAsync(IMessageSession messageSession, Message message, CancellationToken ct = default(CancellationToken))
        {
            // Process the message.
            Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

            await Task.CompletedTask;
        }

        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
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
