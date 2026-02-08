from entityGenerator import generate_person_entity, generate_person_contact_info

from azure.cosmos.aio import CosmosClient
from azure.cosmos import PartitionKey
from configparser import ConfigParser
#from faker import Faker

import os
import json
import uuid
from datetime import datetime, timezone
import time
import sys
import asyncio

parser = ConfigParser()
parser.read('../NotebookConfig.cfg')

cosmosAccountURI = parser.get('CosmosDB', 'COSMOSDB2_ACCOUNT_URI')
cosmosAccountKey = parser.get('CosmosDB', 'COSMOSDB2_ACCOUNT_KEY')

databaseName = 'DemoDB'
containerName = 'entity'
partitionKeypath = '/entityId'

print(cosmosAccountURI)


async def insert_entities(container, num_entities: int = 1000):
    """
    Insert multiple pairs of person and contact documents into the container asynchronously.
    Processes in batches of 100, waiting for each batch to complete before moving to next.
    
    Args:
        container: The async Cosmos DB container to insert into
        num_entities (int): Number of entity pairs to insert
    """
    batch_size = 10
    tasks = []
    completed_batches = 0
    
    for i in range(num_entities):
        entity_id = str(uuid.uuid4())
        
        person_doc = generate_person_entity(entity_id, generate_large_payload=True)
        contact_doc = generate_person_contact_info(entity_id)
        
        # Create async tasks for both documents
        tasks.append(container.upsert_item(person_doc))
        tasks.append(container.upsert_item(contact_doc))
        
        # When we reach batch size, wait for all tasks to complete
        if len(tasks) >= batch_size * 2:  # batch_size pairs = batch_size * 2 documents
            await asyncio.gather(*tasks)
            completed_batches += batch_size
            print(f"Inserted {completed_batches} entity pairs...")
            tasks = []
    
    # Wait for any remaining tasks
    if tasks:
        await asyncio.gather(*tasks)
        print(f"Inserted {num_entities} entity pairs...")
    
    print(f"Successfully inserted {num_entities} entity pairs")


async def main():
    """
    Main function to populate entities in Cosmos DB.
    """
    # Get number of entities from command line argument or use default
    num_entities = int(sys.argv[1]) if len(sys.argv) > 1 else 1000
    
    print(f"Starting insertion of {num_entities} entity pairs...")
    
    async with CosmosClient(cosmosAccountURI, cosmosAccountKey) as client:
        db = client.get_database_client(databaseName)
        ctr = db.get_container_client(containerName)
        
        await insert_entities(ctr, num_entities)
    
    print("Insertion complete!")


if __name__ == "__main__":
    asyncio.run(main())
    
