using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RedisExample
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new RedisClient("redis-18081.c114.us-east-1-4.ec2.cloud.redislabs.com", 18081, "1qc036dKiiinGcYfUjvy7wBdO8pUjCzx"))
            {
                if (args.Count() < 1)
                {
                    //JSON Format -- Easier
                    client.SetEntryInHash("es-MX", "Reg-NotFound", "Error: no encontrado");
                    client.SetEntryInHash("es-MX", "Reg-Duplicate", "Error: este registro ya existe");

                    ////Multiple Values JSON Format
                    //client.SetEntryInHash("es-MX", "Reg", "{\"Reg-NotFound\":\"Error: no encontrado\"},{\"Reg-Duplicate\":\"Error: este registro ya existe\"}");
                    Console.WriteLine("Utiliza Argumentos. Ejemplo: 'dotnet run {ErrorCode}'");
                    Console.WriteLine("---Presiona una tecla para salir---");
                    Console.ReadKey();
                }
                else
                {
                    ////Hash with multiple values
                    //var data = client.GetValueFromHash("es-US",args[0]);
                    //var deserializeData = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
                    //var list = deserializeData.Where(p => p.Key.Contains(args[0]));
                    //List<ErrorModel> lista = list.Select(p => new ErrorModel { Code = p.Key, Message = p.Value }).ToList();
                    //lista.ForEach(error => Console.WriteLine($"{error.Code} => {error.Message}"));
                    //Console.ReadKey();


                    //Hash JSON format
                    var data = client.GetAllEntriesFromHash("es-MX");
                    var list = data.Where(p => p.Key.Contains(args[0]));
                    List<ErrorModel> lista = list.Select(p => new ErrorModel { Code = p.Key, Message = p.Value }).ToList();
                    lista.ForEach(error => Console.WriteLine($"{error.Code} => {error.Message}"));

                    Console.WriteLine("---Presiona una tecla para salir---");
                    Console.ReadKey();
                }
            }
        }
    }
}
