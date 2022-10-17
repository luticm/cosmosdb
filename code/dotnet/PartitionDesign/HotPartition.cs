using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace PartitionDesign
{
    internal class HotPartition
    {
        private string databaseName;
        private string containerName;

        public HotPartition() : this("Learn", "HotPartition")
        {
        }

        public HotPartition(string databaseName, string containerName)
        {
            this.databaseName = databaseName;
            this.containerName = containerName;
        }

        /// <summary>
        /// Force a hot partition scenario. 50% write workload targeted to a single logical partition.
        /// </summary>
        /// <returns></returns>
        public async Task CreateStructure_LoadDocs()
        {
            CosmosClient cc = Config.InitializeClient();
            try
            {

                DatabaseResponse dbResp = await cc.CreateDatabaseIfNotExistsAsync(databaseName);

                // 5 partition container 30000 / 6000
                // Reference URL: 
                ContainerResponse ctrResp = await dbResp.Database.CreateContainerIfNotExistsAsync(containerName, "/Pk", 30000);

                Container c = ctrResp.Container;

                // Force small throughput - each partition 200 RUs
                ThroughputResponse tr = await c.ReplaceThroughputAsync(1000);

                String newId;
                SimpleEntity d;
                ItemResponse<SimpleEntity> ird;

                for (int i = 0; i < 200000; i++)
                {
                    newId = Guid.NewGuid().ToString();

                    // 50% of the workload has PK = 0
                    d = new SimpleEntity(newId, (((i % 100) < 50) ? 0 : (i % 100)).ToString(), newId, newId, null);

                    // Every 100 request, make an async call to avoid excessive throttling
                    if (i % 100 == 0)
                    {
                        ird = await c.CreateItemAsync<SimpleEntity>(d);
                        Console.WriteLine("Writing simple entities: {0}", i.ToString());
                    }
                    else
                    {
                        var response = c.CreateItemAsync<SimpleEntity>(d);
                    }
                }
            }
            catch (Exception ex)
            {
                // Exception handling ...
                throw;
            }
        }

        public async Task CleanUpContainer()
        {
            CosmosClient cc = Config.InitializeClient();
            try
            {
                DatabaseResponse dbResp = await cc.CreateDatabaseIfNotExistsAsync(databaseName);
                Container c = cc.GetContainer(this.databaseName, this.containerName);
                var resp = await c.DeleteContainerAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
