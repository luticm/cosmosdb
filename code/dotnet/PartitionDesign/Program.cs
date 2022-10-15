// See https://aka.ms/new-console-template for more information
using Microsoft.Azure.Cosmos;
using PartitionDesign;

CosmosClient cc = Config.InitializeClient();
Console.WriteLine("Choose an option. 0 to exit:");
Console.WriteLine("1 - Hot partition");

int option;
while (true) { 
    Int32.TryParse(Console.ReadLine(), out option);
    Console.WriteLine(option);

    if (option == 0)
    {
        Console.WriteLine("0 or invalid option. Exiting...");
        break;
    }

    switch (option) { 
    case 1: 
        Console.WriteLine("option 1");
        break;  
    }
}





