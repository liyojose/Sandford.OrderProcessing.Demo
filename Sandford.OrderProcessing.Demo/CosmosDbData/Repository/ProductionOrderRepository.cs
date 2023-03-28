
using Microsoft.Azure.Cosmos;
using Sandford.OrderProcessing.Demo.CosmosDbData.Base;
using Sandford.OrderProcessing.Demo.CosmosDbData.Entity;
using Sandford.OrderProcessing.Demo.CosmosDbData.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sandford.OrderProcessing.Demo.CosmosDbData.Repository
{

    public interface IProductionOrderRepository : IRepository<ProductionOrder>
    {
    }
    public class ProductionOrderRepository : CosmosDbRepository<ProductionOrder>, IProductionOrderRepository
    {
        /// <summary>
        ///     CosmosDB container name
        /// </summary>
        public override string ContainerName { get; } = "purchaseorder";

        /// <summary>
        ///     Generate Id.
        ///     e.g. "shoppinglist:783dfe25-7ece-4f0b-885e-c0ea72135942"
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        //public override string GenerateId(ProductionOrder entity) => $"{entity.Id}:{Guid.NewGuid()}";
        public override string GenerateId(ProductionOrder entity) => $"{entity.Id}";

        /// <summary>
        ///     Returns the value of the partition key
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public override PartitionKey ResolvePartitionKey(string entityId) => new PartitionKey(entityId.Split(':')[0]);

        public ProductionOrderRepository(ICosmosDbContainerFactory factory) : base(factory)
        { }

        
    }
}
