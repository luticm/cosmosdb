using System;
using System.Threading.Tasks;

namespace CosmosGlobal
{
    internal class Program
    {
        // Pre-work: create account close to your region (ex.: SCUS) and another remote one South India.
        public static async Task Main(string[] args)
        {
            EntityHandler eh = new EntityHandler();
            //await eh.CreateStructure();
            //await eh.LoadDocs();
            await eh.SimpleWorkload();


            Console.WriteLine("Execution finished!");
        }

        private static async Task Execute() { 
        }
    }
}
