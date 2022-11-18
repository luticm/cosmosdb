using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Numerics;
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
        private CosmosClient cc;

        public HotPartition() : this("Learn", "HotPartition")
        {
        }

        public HotPartition(string databaseName, string containerName)
        {
            this.databaseName = databaseName;
            this.containerName = containerName;

            this.cc = Config.InitializeClient();
        }

        /// <summary>
        /// Force a hot partition scenario. 50% write workload targeted to a single logical partition.
        /// </summary>
        /// <returns></returns>
        public async Task CreateStructure_LoadDocs()
        {  
            try
            {
                DatabaseResponse dbResp = await cc.CreateDatabaseIfNotExistsAsync(databaseName);

                // 5 partition container 30000 / 6000
                // Reference URL: 
                ContainerResponse ctrResp = await dbResp.Database.CreateContainerIfNotExistsAsync(containerName, "/Pk", 30000);

                Container c = ctrResp.Container;

                // Force small throughput - each partition 400 RUs
                ThroughputResponse tr = await c.ReplaceThroughputAsync(2000);
            }
            catch (Exception ex)
            {
                // Exception handling ...
                throw;
            }
        }

        public async Task LoadDocs() {
            try
            {
                Container c = cc.GetContainer(databaseName, containerName);

                String newId;
                SimpleEntity d;
                ItemResponse<SimpleEntity> ird;

                for (int i = 0; i < 200000; i++)
                {
                    newId = Guid.NewGuid().ToString();

                    // 50% of the workload has PK = 0
                    d = new SimpleEntity(newId, (((i % 100) < 50) ? 0 : (i % 100)).ToString(), newId, newId, null);

                    // Every 100 request, make an async call to avoid excessive throttling
                    if (i % 50 == 0)
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

        // Demonstrate single PK and cross partition queries
        public async Task SimpleQuery()
        {
            Container c = cc.GetContainer(databaseName, containerName);
            using StreamWriter sw = new StreamWriter(@"c:\temp\QueryOutput_DiagnosticsSample.txt");
            List<SimpleEntity> resultado = new List<SimpleEntity>();

            try
            {
                QueryDefinition query = new QueryDefinition("SELECT * FROM c WHERE c.Pk = '51'");
                using (FeedIterator<SimpleEntity> rsIterator = c.GetItemQueryIterator<SimpleEntity>(query
                    , requestOptions: new QueryRequestOptions() { PartitionKey = new PartitionKey("51"), MaxItemCount = 1000 }))
                {
                    while (rsIterator.HasMoreResults)
                    {
                        FeedResponse<SimpleEntity> response = await rsIterator.ReadNextAsync();
                        resultado.AddRange(response);
                        if (response.Diagnostics != null)
                        {
                            await sw.WriteLineAsync(response.Diagnostics.ToString());

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Exception handling ...
                throw;
            }
        }

        public async Task SimpleQueryCrossPK()
        {
            Container c = cc.GetContainer(databaseName, containerName);
            using StreamWriter sw = new StreamWriter(@"c:\temp\QueryOutputCrossPK_DiagnosticsSample.txt");
            List<SimpleEntity> resultado = new List<SimpleEntity>();

            try
            {
                QueryDefinition query = new QueryDefinition("SELECT * FROM c WHERE c.Pk in ('51', '52')");
                using (FeedIterator<SimpleEntity> rsIterator = c.GetItemQueryIterator<SimpleEntity>(query
                    , requestOptions: new QueryRequestOptions() { MaxItemCount = 1000 }))
                {
                    while (rsIterator.HasMoreResults)
                    {
                        FeedResponse<SimpleEntity> response = await rsIterator.ReadNextAsync();
                        resultado.AddRange(response);
                        if (response.Diagnostics != null)
                        {
                            await sw.WriteLineAsync(response.Diagnostics.ToString());

                        }
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
