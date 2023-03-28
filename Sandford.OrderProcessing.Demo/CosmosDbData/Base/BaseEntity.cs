using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandford.OrderProcessing.Demo.CosmosDbData.Base
{
    public abstract class BaseEntity
    {
        //[JsonProperty("id")]
        [JsonProperty(PropertyName = "id")]
        public virtual string Id { get; set; }
    }
}
