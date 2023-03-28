
using Sandford.OrderProcessing.Demo.CosmosDbData.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandford.OrderProcessing.Demo.CosmosDbData.Entity
{
    public class ProductionOrder : BaseEntity
    {
        public Guid SalesOrderId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ProductionStatus Status { get; set; }
        public List<ProductionOrderItem> Items { get; set; }
    }

    public class ProductionOrderItem
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public ProductionStatus Status { get; set; }
    }

    public enum ProductionStatus
    {
        NotStarted,
        InProgress,
        Completed,
        Cancelled
    }
}
