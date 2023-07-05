using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartitionDesign
{
    internal class BurstCapacity
    {
        private string databaseName;
        private string containerName;
        private CosmosClient cc;

        public BurstCapacity() : this("Learn", "Burst")
        {
        }

        public BurstCapacity(string databaseName, string containerName)
        {
            this.databaseName = databaseName;
            this.containerName = containerName;

            this.cc = Config.InitializeClient();
        }
        public async Task PrepareStructure()
        {
            try
            {
                DatabaseResponse dbResp = await cc.CreateDatabaseIfNotExistsAsync(databaseName);

                // 5 partition container (30000 / 6000)
                ContainerResponse ctrResp = await dbResp.Database.CreateContainerIfNotExistsAsync(containerName+"5pp", "/Pk", 30000);
                
                // Small throughput - each partition 800 RUs
                ThroughputResponse tr = await ctrResp.Container.ReplaceThroughputAsync(4000);

                // 2nd container, single physical partition
                ctrResp = await dbResp.Database.CreateContainerIfNotExistsAsync(containerName + "1pp", "/Pk", 4000);

                // Small throughput - single partition with 4K RUs
                tr = await ctrResp.Container.ReplaceThroughputAsync(4000);
            }
            catch (Exception ex)
            {
                // Exception handling ...
                throw;
            }
        }

        public async Task LoadDocs(int pauseInSeconds = 300, int numElements = 10000, string collSuffix = "1pp")
        {
            try
            {
                Container container = cc.GetContainer(databaseName, containerName + collSuffix);
               
                //SimpleEntityExpanded sed;
                ItemResponse<SimpleEntityExpanded> ird;

                while (true) {

                    Console.WriteLine("Preparing entities");
                    SimpleEntityExpanded[] objectArray = new SimpleEntityExpanded[numElements];
                    for (int i = 0; i < numElements; i++)
                    {
                        objectArray[i] = new SimpleEntityExpanded();
                    }

                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    for (int i = 0; i < numElements; i++)
                    {
                        // Every 200 request, make an async call to avoid excessive throttling
                        if (i % 100 == 0)
                        {
                            ird = await container.CreateItemAsync<SimpleEntityExpanded>(objectArray[i]);

                            if (i % 1000 == 0)
                                Console.WriteLine("Writing simple entities: {0}", i.ToString());
                        }
                        else
                        {
                            // Fire and forget
                            container.CreateItemAsync<SimpleEntityExpanded>(objectArray[i]);
                        }
                    }

                    sw.Stop();
                    Console.WriteLine("Completed a batch in {0} seconds, waiting for {1} seconds.", sw.Elapsed.TotalSeconds, pauseInSeconds);
                    System.Threading.Thread.Sleep(pauseInSeconds * 1000);
                }
            }
            catch (Exception ex)
            {
                // Exception handling ...
                throw;
            }
        }
    }
}
