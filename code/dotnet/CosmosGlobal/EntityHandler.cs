using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CosmosGlobal
{
    internal class EntityHandler
    {
        private string databaseName;
        private string containerName;
        private CosmosClient cc;

        public EntityHandler() : this("Learn", "GlobalDist")
        {
        }

        public EntityHandler(string databaseName, string containerName)
        {
            this.databaseName = databaseName;
            this.containerName = containerName;

            this.cc = Config.InitializeClient();
            //this.cc = Config.InitializeClientConsistencyLvl();
            //this.cc = Config.InitializeClientPreferredRegion();
        }

        /// <summary>
        /// Force a hot partition scenario. 50% write workload targeted to a single logical partition.
        /// </summary>
        /// <returns></returns>
        public async Task CreateStructure()
        {
            try
            {
                DatabaseResponse dbResp = await cc.CreateDatabaseIfNotExistsAsync(databaseName);

                ContainerResponse ctrResp = await dbResp.Database.CreateContainerIfNotExistsAsync(containerName, "/Pk", 400);

                Container c = ctrResp.Container;
            }
            catch (Exception ex)
            {
                // Exception handling ...
                throw;
            }
        }

        public async Task LoadDocs()
        {
            try
            {
                Container c = cc.GetContainer(databaseName, containerName);

                String newId;
                SimpleEntity d;
                ItemResponse<SimpleEntity> ird;

                for (int i = 0; i < 100; i++)
                {
                    newId = Guid.NewGuid().ToString();
                    d = new SimpleEntity(i.ToString(), i.ToString(), newId, newId, null);

                    // Every 10 request, make an async call to avoid excessive throttling
                    if (i % 10 == 0)
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

        // 50 cross region calls
        public async Task SimpleWorkload()
        {
            Container c = cc.GetContainer(databaseName, containerName);
            ItemResponse<SimpleEntity> ir;
            
            using StreamWriter sw = new StreamWriter(@"c:\temp\PointReadsGlobalDemo_DiagnosticsSample.txt");

            for (int i = 0; i < 50; i++) {
                ir = await c.ReadItemAsync<SimpleEntity>(i.ToString(), new PartitionKey(i.ToString()));
                Console.WriteLine("Round trip time: " + ir.Diagnostics.GetClientElapsedTime().ToString());
                sw.WriteLine(ir.Diagnostics);
            }
        }

    }
}
