using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PartitionDesign
{
    internal class IntroToSDK
    {
        private string databaseName = "Learn";
        private string containerName = "SDK";
        private CosmosClient cc;

        public IntroToSDK()
        {
            cc = Config.InitializeClient();
        }

        public async Task CreateStructure()
        {
            try
            {
                DatabaseResponse dbResp = await cc.CreateDatabaseIfNotExistsAsync(databaseName);
                ContainerResponse ctrResp = await dbResp.Database.CreateContainerIfNotExistsAsync(containerName, "/Pk", 400);

                Container c = ctrResp.Container;

                // Force small throughput - each partition 400 RUs
                ThroughputResponse tr = await c.ReplaceThroughputAsync(1000);
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

                for (int i = 0; i < 1000; i++)
                {
                    newId = Guid.NewGuid().ToString();

                    // 50% of the workload has PK = 0
                    d = new SimpleEntity(newId, (i % 100).ToString(), newId, newId, null);

                    // Every 100 request, make an async call to avoid excessive throttling
                    if (i % 20 == 0)
                    {
                        ird = await c.CreateItemAsync<SimpleEntity>(d);
                        Console.WriteLine("Writing simple entities: {0}", i.ToString());
                    }
                    else
                    {
                        var response = c.CreateItemAsync<SimpleEntity>(d);
                    }
                }

                for (int i = 0; i < 10; i++)
                {
                    newId = i.ToString();

                    // 50% of the workload has PK = 0
                    d = new SimpleEntity(newId, (i % 100).ToString(), newId, newId, null);
                    ird = await c.CreateItemAsync<SimpleEntity>(d);
                }
            }
            catch (Exception ex)
            {
                // Exception handling ...
                throw;
            }
        }

        public async Task PointRead()
        {
            try
            {
                Container c = cc.GetContainer(databaseName, containerName);

                using StreamWriter sw = new StreamWriter("PointRead.txt");
                var resp = await c.ReadItemAsync<SimpleEntity>("1", new PartitionKey("1"));

                sw.WriteLine(resp.Diagnostics.ToString());
                Console.WriteLine(resp.RequestCharge.ToString());
            }
            catch (Exception ex)
            {
                // Exception handling ...
                throw;
            }
        }

        public async Task PointQuery()
        {
            try
            {
                Container c = cc.GetContainer(databaseName, containerName);

                using StreamWriter sw = new StreamWriter("QueryOutput.txt");

                QueryDefinition query = new QueryDefinition("SELECT * FROM c WHERE c.id = '1' and c.pk = '1'");
                using (FeedIterator<dynamic> rsIterator = c.GetItemQueryIterator<dynamic>(query
                    , requestOptions: new QueryRequestOptions() { MaxItemCount = 100 }))
                {
                    while (rsIterator.HasMoreResults)
                    {
                        FeedResponse<dynamic> response = await rsIterator.ReadNextAsync();                        
                        if (response.Diagnostics != null)
                        {
                            await sw.WriteLineAsync(response.Diagnostics.ToString());
                            Console.WriteLine(response.RequestCharge.ToString());
                            
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

    }
}
