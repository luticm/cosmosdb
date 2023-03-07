using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace CosmosGlobal
{
    internal class Config
    {
        public static string cosmosEndpoint = "";
        public static string cosmosKey = "";
        private static Microsoft.Azure.Cosmos.CosmosClient cosmosClient;

        private Config() { }

        // Read configuration from appsettings.json
        //     ==> gitignore, add your file to the project
        // PM> install-package Microsoft.Extensions.Configuration
        // PM> install-package Microsoft.Extensions.Configuration.Json        
        static Config()
        {
            IConfigurationBuilder cb = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true);
            IConfiguration config = cb.Build();

            cosmosEndpoint = config["CosmosEndpoint"];
            if (cosmosEndpoint == null)
            { cosmosEndpoint = "Hardcoded endpoint"; }

            cosmosKey = config["CosmosKey"];
            if (cosmosKey == null)
            { cosmosKey = "Hardcoded Cosmos DB key"; }
        }


        public static CosmosClient InitializeClient()
        {
            if (cosmosClient == null)
            {
                cosmosClient = new CosmosClient(cosmosEndpoint, cosmosKey);
            }
            return cosmosClient;
        }
        public static CosmosClient InitializeClientConsistencyLvl()
        {
            if (cosmosClient == null)
            {
                CosmosClientOptions cco = new CosmosClientOptions();
                cco.ConsistencyLevel = ConsistencyLevel.Session;
                cosmosClient = new CosmosClient(cosmosEndpoint, cosmosKey, cco);
            }
            return cosmosClient;
        }

        public static CosmosClient InitializeClientGateway()
        {
            if (cosmosClient == null)
            {
                cosmosClient = new CosmosClient(cosmosEndpoint, cosmosKey, new CosmosClientOptions() { ConnectionMode = ConnectionMode.Gateway });
            }
            return cosmosClient;
        }

        public static CosmosClient InitializeClientPreferredRegion()
        {
            if (cosmosClient == null)
            {
                CosmosClientOptions cco = new CosmosClientOptions();
                cco.ConsistencyLevel = ConsistencyLevel.BoundedStaleness;
                cco.ApplicationPreferredRegions= new List<string>() {Regions.SouthIndia, Regions.SouthCentralUS};
                cosmosClient = new CosmosClient(cosmosEndpoint, cosmosKey, cco);
            }
            return cosmosClient;
        }
    }
}
