using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartitionDesign
{
    internal class HotPartitionTime
    {
        private string databaseName;
        private string containerName;

        public HotPartitionTime() : this("Learn", "HotPartitionTime")
        {
        }

        public HotPartitionTime(string databaseName, string containerName)
        {
            this.databaseName = databaseName;
            this.containerName = containerName;
        }

        public async Task CreateStructure_LoadDocs()
        {
            CosmosClient cc = Config.InitializeClient();
            try
            {

                DatabaseResponse dbResp = await cc.CreateDatabaseIfNotExistsAsync(databaseName);

                // 5 partition container 30000 / 6000
                // Reference URL: 
                ContainerResponse ctrResp = await dbResp.Database.CreateContainerIfNotExistsAsync(containerName, "/PartitionKey", 30000);
                Container c = ctrResp.Container;

                // Define smaller throughput - each partition 200 RUs
                ThroughputResponse tr = await c.ReplaceThroughputAsync(1000);

                DateTime baseTime = new DateTime(2022, 11, 01);
                String readingId;
                string timeRange;
                SimpleSensor ss;
                ItemResponse<SimpleSensor> ird;

                // 50 sensors, reading every minute
                for (int j = 0; j < 100000; j++) {
                    for (int i = 1; i <= 50; i++)
                    {
                        timeRange = baseTime.AddSeconds(j).ToString("yyyy-MM-dd-HH-mm");
                        readingId = Guid.NewGuid().ToString();
                        ss = new SimpleSensor(i.ToString(), readingId, timeRange);                        

                        // Every 25 requests, make an async call to avoid excessive throttling
                        if (i % 25 == 0)
                        {
                            ird = await c.CreateItemAsync<SimpleSensor>(ss);
                            Console.WriteLine("Writing simple entities: {0}", i.ToString());
                        }
                        else
                        {
                            var response = c.CreateItemAsync<SimpleSensor>(ss);
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
