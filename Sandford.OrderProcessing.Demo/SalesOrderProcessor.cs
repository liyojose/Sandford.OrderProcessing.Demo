using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandford.OrderProcessing.Demo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Microsoft.Azure.Cosmos;
    using Sandford.OrderProcessing.Demo.CosmosDbData.Entity;
    using Sandford.OrderProcessing.Demo.CosmosDbData.Repository;
    using Sandford.OrderProcessing.Demo.Dto;
    using Container = Microsoft.Azure.Cosmos.Container;

    public interface ISalesOrderProcessor
    {
        Task ProcessSalesOrderAsync(SalesOrder salesOrder);
    }

    public class SalesOrderProcessor : ISalesOrderProcessor
    {
        private readonly IProductionOrderRepository _repo;

        public SalesOrderProcessor(IProductionOrderRepository repo)
        {
            _repo = repo;
        }

        public async Task ProcessSalesOrderAsync(SalesOrder salesOrder)
        {
            if (salesOrder == null)
            {
                throw new ArgumentNullException(nameof(salesOrder));
            }

            if (salesOrder.Items == null || salesOrder.Items.Count == 0)
            {
                throw new ArgumentException("Sales order must contain at least one item.");
            }

            // Validate data for each item in the sales order
            foreach (var item in salesOrder.Items)
            {
                if (item.ProductId == Guid.Empty)
                {
                    throw new ArgumentException("Invalid product ID in sales order item.");
                }

                if (item.Quantity <= 0)
                {
                    throw new ArgumentException("Invalid quantity in sales order item.");
                }

                if (item.Price <= 0)
                {
                    throw new ArgumentException("Invalid price in sales order item.");
                }
            }

            // Calculate production start and end dates
            DateTime startDate = DateTime.Today.AddDays(1);
            DateTime endDate = startDate.AddDays(7);

            // Convert sales order to production order
            var productionOrder = new ProductionOrder
            {
                Id = Guid.NewGuid().ToString(),
                SalesOrderId = salesOrder.OrderId,
                StartDate = startDate,
                EndDate = endDate,
                Status = ProductionStatus.NotStarted,
                Items = new List<ProductionOrderItem>()
            };

            foreach (var item in salesOrder.Items)
            {
                var productionItem = new ProductionOrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Status = ProductionStatus.NotStarted
                };

                productionOrder.Items.Add(productionItem);
            }

            // Insert production order into Cosmos DB
            await _repo.AddItemAsync(productionOrder);
        }
    }

}
