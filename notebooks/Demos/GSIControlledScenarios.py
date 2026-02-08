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

databaseName = 'BBashDB'
containerName = 'entity'
partitionKeypath = '/entityId'

print(cosmosAccountURI)