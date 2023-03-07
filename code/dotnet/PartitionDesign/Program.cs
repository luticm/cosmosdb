// See https://aka.ms/new-console-template for more information
using Microsoft.Azure.Cosmos;
using PartitionDesign;

//CosmosClient cc = Config.InitializeClient();
HotPartition ht = new HotPartition();
HotPartitionTime htt = new HotPartitionTime();
IntroToSDK its = new IntroToSDK();

int option;
while (true) {
    Console.WriteLine("");
    Console.WriteLine("Choose an option. 0 to exit:");
    Console.WriteLine("0 - Intro to SDK");
    Console.WriteLine("1 - Hot partition");
    Console.WriteLine("2 - Hot partition time");
    Console.WriteLine("8 - Quick test");
    Console.WriteLine("9 - Clean up");

    Int32.TryParse(Console.ReadLine(), out option);
    Console.WriteLine("");

    if (option == 0)
    {
        Console.WriteLine("0 or invalid option. Exiting...");
        break;
    }

    switch (option) {
        case 7:
            Console.WriteLine("0 - SDK Intro");
            await its.CreateStructure();
            await its.LoadDocs();
            await its.PointRead();
            await its.PointQuery();
            break;
        case 1: 
            Console.WriteLine("1 - Hot partition");
            await ht.CreateStructure_LoadDocs();
            await ht.LoadDocs();
            break;
        case 2:
            Console.WriteLine("2 - Hot partition time");
            await htt.CreateStructure_LoadDocs();
            break;
        case 8:
            Console.WriteLine("8 - Quick test");
            await ht.SimpleQuery();
            await ht.SimpleQueryCrossPK();
            break;
        case 9:
            Console.WriteLine("9 - Clean up");
            await ht.CleanUpContainer();
            await htt.CleanUpContainer();
            break;
    }
}





