## Notebooks
- NoSQL
  - Create base containers with 5 and 10 physical partitions - [notebook](Models/PrepareContainers.ipynb)
  - [Geospatial example](Models/GeoJSON.ipynb)
- MongoDB
- Work in Progress (WIP)
  - asdf


### Basic usage

- Configuration settings are stored in NotebookConfig.cfg (Cosmos DB URL, keys, etc.)
  - INI file structure used by ConfigParser (Python) and Microsoft.Extensions.Configuration.Ini (.NET)
  - Part of .gitignore
  - Sample NotebookConfig.cfg with format:<br/>

    [CosmosDB]<br/>
    COSMOSDB_ACCOUNT_URI: https://CosmosDBAccount.documents.azure.com:443/<br/>
    COSMOSDB_ACCOUNT_KEY: CosmosDBKey<br/>

- Installing libraries
  - %pip install azure-cosmos
  - %pip install faker 
  
<br/>

- Python Faker library
  - https://faker.readthedocs.io/en/master/index.html