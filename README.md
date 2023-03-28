Prerequisites
1)	Azure Service bus Queue, Azure CosmosDB, Azure Key Vault, Azure Service Bus

2)	Azure Service bus Queue
a.	Create a service bus queue and get the connection string.
b.	Store the connection string in Azure Key vault. 
c.	Use managed identity and use Azure key Vault referencing in Azure settings. https://learn.microsoft.com/en-us/azure/app-service/app-service-key-vault-references?tabs=azure-cli
d.	
3)	Azure Cosmos DB
a.	Create DB (Order) and create a container (purchaseorder) 
b.	Get the connection and key details and update in azure appsettings. 
c.	Use Azure Key Vault and Key Vault referencing in portal to store connection strings. 

Solution Architecture
This demonstrates the different layers of a Clean Architecture solution:
•	Core, which defines the business entities, business logic abstractions, interfaces, etc..
•	Infrastructure, which provides the implementations like NoSQL data storage, EF Core, Repository implementation, etc..
Azure Functions project
This project is Azure Functions 4.0 targeting .Core 6.0 There are just a few files within the project because other supporting files reside in Core project or Infrastructure.
host.json, which has runtime settings for application level, such as timeout setting.
local.settings.json, which is similar to appsettings.json in an ASP.NET Core Web project, includes configuration settings for service bus, CosmosDB
SalesOrderProcessingFunction.cs, which is a Service Bus Queue triggered function.
Startup.cs, which is similar to the Startup.cs class in an ASP.NET Core Web project, register the services and their lifetime throughout the application using Microsoft built-in dependency injection container. 
Let’s explain some major pieces in the code.

Startup.cs

A few take away notes on this class are:

The FunctionsStartup assembly attribute is added to specify that Startup.cs should run during startup, which is kind of like the Main() method being the entry point at startup in a Console application.
For configurations, Json file local.settings.json is added as one of the configuration providers.
The database related settings like connection strings, database name, container names and partition keys are stored in local.settings.json, and are read into configuration object.
AddCosmosDB() is an extension method defined in the Infrastructure project so that it can be used in both the API project and the Azure Functions project. AddCosmosDB() registers a singleton instance of CosmosDbContainerFactory, which is a wrapper class for CosmosClient provided by Cosmos DB .NET V3. According to Microsoft documentation on CosmosClient, a single instance of CosmosClient should be used. 
Because we are using repository pattern, we also register the ProductionOrderRepository, which will internally know what Cosmos DB container to use and what the partition key value is.


Local.settings.json

{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "SBConnectionSaleOrderQueue": "your connection string.. Only for devloment", //Always update in Azure Keyvault and read in appsetting using keyvault refernces.
    "SalesOrderQueue": "salesorder" //put your message queue  name here
  },
  "ConnectionStrings": {
    "CosmosDB": {
      "EndpointUrl": "",
      // default primary key used by CosmosDB emulator
      "PrimaryKey": "uCvfiYT6ba1lRqqyquJ8IeNDLzkWTxTtfYFYWze7JyIPYJdWNv72M9toJGjlCUoveXx6iYlr5sIxACDbCrNUdw==",//Always update in Azure Keyvault and read in appsetting using keyvault refernces.
      "DatabaseName": "Order",
      "Containers": [
        {
          "Name": "purchaseorder",
          "PartitionKey": "/Id"
        }
      ]
    }
  }
}

Host.json
{
  "version": "2.0",
  "logging": {
    "applicationInsights": {
      "samplingExcludedTypes": "Request",
      "samplingSettings": {
        "isEnabled": true
      }
    }
  },
  "extensions": {
    "serviceBus": {
      "messageHandlerOptions": {
        "autoComplete": false,
        "maxConcurrentCalls": 1
      }
    }
  }
}

Deployment

Azure function can be deployed to azure environment by either setting up CICD or you can import the publish profile from Azure portal and import in Visual Studio and publish. Once published make sure all config values are in place. 
