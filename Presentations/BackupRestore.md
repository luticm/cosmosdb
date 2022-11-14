## Backup/Restore

- **Periodic backup**: https://learn.microsoft.com/en-us/azure/cosmos-db/configure-periodic-backup-restore
  - If the container or database is deleted, Azure Cosmos DB retains the existing snapshots of a given container or database for 30 days
  - Restore process: https://learn.microsoft.com/en-us/azure/cosmos-db/configure-periodic-backup-restore#request-restore
     - The restored account will have the same provisioned throughput, indexing policies and it is in same region as the original account. (https://learn.microsoft.com/en-us/azure/cosmos-db/configure-periodic-backup-restore#post-restore-actions)
  - Pricing: https://azure.microsoft.com/en-us/pricing/details/cosmos-db/standard-provisioned/

- **Continuous backup**: https://learn.microsoft.com/en-us/azure/cosmos-db/continuous-backup-restore-introduction
  - All mutations performed on the source account (which includes databases, containers, and items) are backed up asynchronously within 100 seconds
  - Pricing: https://learn.microsoft.com/en-us/azure/cosmos-db/continuous-backup-restore-introduction#continuous-backup-pricing
  - Latest restorable: 
    - az cosmosdb sql retrieve-latest-backup-time -g cosmicgbb-datalz -a cosmicgbb-sql -d Learn -c HotPartitionTime -l 'South Central US'
    - For DB/Account, check scripts: https://learn.microsoft.com/en-us/azure/cosmos-db/get-latest-restore-timestamp

- What isn't restored: https://learn.microsoft.com/en-us/azure/cosmos-db/continuous-backup-restore-introduction#what-isnt-restored
- Can move from periodic to continuous, but not the other way around
- Restoring 1 terabyte of data typically takes 30 to 90 minutes: https://learn.microsoft.com/en-us/azure/cosmos-db/continuous-backup-restore-frequently-asked-questions#how-much-time-does-it-takes-to-restore-
