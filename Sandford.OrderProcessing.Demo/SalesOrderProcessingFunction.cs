using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sandford.OrderProcessing.Demo.Dto;

namespace Sandford.OrderProcessing.Demo
{
    public class SalesOrderProcessingFunction
    {

        private readonly ISalesOrderProcessor _processor;
        private readonly ILogger<SalesOrderProcessingFunction> _log;
        public SalesOrderProcessingFunction(ISalesOrderProcessor processor, ILogger<SalesOrderProcessingFunction> log)
        {
            _processor = processor;
            _log = log;
        }

        [FunctionName("Process")]
        public async Task Run([ServiceBusTrigger("SalesOrder", Connection = "SBConnectionSaleOrderQueue")] Message message, MessageReceiver messageReceiver,ILogger log)
        {
            try
            {
               // var od = new SalesOrder
               // {
               //     CustomerId = Guid.NewGuid(),
               //     OrderDate = DateTime.Now,
               //     OrderId = Guid.NewGuid(),
               //     Items = new System.Collections.Generic.List<OrderItem> { new OrderItem { Price = 10, ProductId = Guid.NewGuid(), Quantity = 10 } }
               // };
               //var stringod = JsonConvert.SerializeObject(od);

                _log.LogInformation($"C# ServiceBus queue trigger function processed message: {message.Body}");
                var order = JsonConvert.DeserializeObject<SalesOrder>(Encoding.UTF8.GetString(message.Body));
                await _processor.ProcessSalesOrderAsync(order);
                //complete the message if there is no error
               await  messageReceiver.CompleteAsync(message.SystemProperties.LockToken);
            }
            catch (Exception ex)
            {
                _log.LogError($"Error occured while processing order.Exception: {ex.Message}");
                // Send message to DeadLetter Queue 
                await messageReceiver.DeadLetterAsync(message.SystemProperties.LockToken);

                throw;
            }


        }

    }
}
