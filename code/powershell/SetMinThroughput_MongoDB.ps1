#***************************************************************************************
#   Author: Luciano Moreira
#   Util script to set the MINIMUM throughput for all Cosmos DB for MONGO API in a subscription/resource group.
#       Only for MONGO API and collections (not shared database throughput) (2024-08-07)
#
#   PS: ignore NotFound messages for offer, this is expected if AZ CLI request doesn't throughput config.
# ***************************************************************************************

# az login
# az account set -s "<SubscriptionId>"

Start-Transcript -Path "C:\Temp\SetMinThroughputMongo_$((Get-Date).ToString("yyyyMMddHH")).log"
Write-Output "Starting CosmosDB SetMinThroughput_MongoDB.ps1 script at $((Get-Date).ToString("yyyy-MM-dd HH:mm:ss"))"
write-output "Current Azure Subscription: $(az account show --query '[name, id]' -o json)"

$rgs = $null

# Go thru all RGs
#$rgs = az group list | ConvertFrom-Json | select-object -Property name

# OR filter one specific resource group
$rgs = az group list | ConvertFrom-Json | select-object -Property name | where-object {$_.name -eq "cosmicgbb-datalz"}

# For all resource groups
foreach($rg in $rgs) {
    $rgName = $rg.name
    Write-Output("`n`n`n**** Processing Resource Group: $rgName")

    # Only processing Kind = GlobalDocumentDB
    $accounts = $null
    $accounts = az cosmosdb list -g $rgName | ConvertFrom-Json | select-object -Property name, resourceGroup, id, kind | where-object {$_.kind -eq "MongoDB"}

    #For all Cosmos DB accounts
    foreach($account in $accounts) {
        $accountName = $account.name
        Write-Output("`n`n`n*** Processing Cosmos DB: $accountName")

         $dbs = $null
         $dbs = az cosmosdb mongodb database list -g $account.resourceGroup -a $account.name | ConvertFrom-Json | select-object -Property name

        # For all databases
        foreach($db in $dbs) {
            Write-Output ("`n`n** Processing Database: $($db.name)")

            # Removing shared database throughput for simplicity, let it bubble exception if try to set collection throughput for a shared DB

            $colls = $null
            $colls = az cosmosdb mongodb collection list -g $account.resourceGroup -a $account.name -d $db.name | ConvertFrom-Json | select-object -Property name

            # For all containers
            foreach($coll in $colls) {
                Write-Output ("`n* Processing Container: $($coll.name)")
                $colloffer = $null
                $colloffer = az cosmosdb mongodb collection throughput show -g $account.resourceGroup -a $account.name -d $db.name -n $coll.name | ConvertFrom-Json | select-object -ExpandProperty resource | select-object -Property minimumThroughput, instantMaximumThroughput, throughput, autoscaleSettings
                #Write-Output $colloffer

                if ($colloffer -ne $null) {
                    Write-Output "Container Offer found. $($coll.name) has throughput setting." 
                    $autoscale = $null
                    $autoscale = $colloffer | select-object -ExpandProperty autoscaleSettings

                    if ($autoscale -ne $null) {
                        Write-Output "Autoscale is enabled. Current max: $($autoscale.maxThroughput) `nSuggested autoscale max throughput: $($colloffer.minimumThroughput)"
                        if ($autoscale.maxThroughput -ne $colloffer.minimumThroughput) {
                            Write-Output "Setting autoscale max throughput to: $($colloffer.minimumThroughput)"
                            az cosmosdb mongodb collection throughput update -g $account.resourceGroup -a $account.name -d $db.name -n $coll.name --max-throughput $colloffer.minimumThroughput
                        }
                    }
                    else {
                        Write-Output "Autoscale is disabled. Current throughput: $($colloffer.throughput) `nSuggested container minimum throughput: $($colloffer.minimumThroughput)"
                        if ($colloffer.throughput -ne $colloffer.minimumThroughput) {
                            Write-Output "Setting container throughput to: $($colloffer.minimumThroughput)"
                            az cosmosdb mongodb collection throughput update -g $account.resourceGroup -a $account.name -d $db.name -n $coll.name --throughput $colloffer.minimumThroughput
                        }
                    }
                }
            }
        }
    }
}
Stop-Transcript