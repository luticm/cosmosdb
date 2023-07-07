// See https://aka.ms/new-console-template for more information
using Microsoft.Azure.Cosmos;
using PartitionDesign;

Config.InitializeClientGateway();
HotPartition ht = new HotPartition();
HotPartitionTime htt = new HotPartitionTime();
BurstCapacity bc = new BurstCapacity();
IntroToSDK its = new IntroToSDK();

int option;
while (true) {
    Console.WriteLine("");
    Console.WriteLine("Choose an option. 0 to exit:");
    Console.WriteLine("1 - Hot partition");
    Console.WriteLine("2 - Hot partition time");
    Console.WriteLine("3 - Burst capacity");
    Console.WriteLine("7 - Intro to SDK");
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
        case 1: 
            Console.WriteLine("1 - Hot partition");
            await ht.CreateStructure_LoadDocs();
            await ht.LoadDocs();
            break;
        case 2:
            Console.WriteLine("2 - Hot partition time");
            await htt.CreateStructure_LoadDocs();
            break;
        case 3:
            Console.WriteLine("3 - Burst");
            await bc.PrepareStructure();

            int secondsToWait = 300;
            int numElements = 10000;

            Console.WriteLine("Enter collection suffix: \"1pp\" (default) or \"5pp\")");
            string? collSuffix = Console.ReadLine();

            if (collSuffix != "5pp")
            {
                collSuffix = "1pp";
            }

            Console.WriteLine("Processing with 2 threads. \nConfig: {0} seconds to wait; {1} docs ; {2} collSuffix",
                secondsToWait, numElements, collSuffix);

            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 2; i++)
            {   
                tasks.Add(bc.LoadDocs(secondsToWait, numElements, collSuffix));
            }

            await Task.WhenAll(tasks);
            break;
        case 7:
            Console.WriteLine("7 - SDK Intro");
            await its.CreateStructure();
            //await its.LoadDocs();
            await its.PointRead();
            await its.PointQuery();
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





