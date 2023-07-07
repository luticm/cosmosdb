#***************************************************************************************
#   Author: Luciano Moreira
#   Util script to set the minimum throughput for all Cosmos DB databases and containers in a subscription.
# ***************************************************************************************

# az login
# az account set -s "<SubscriptionId>"

Start-Transcript -Path "C:\Temp\SetMinThroughput_$((Get-Date).ToString("yyyyMMddHH")).log"
Write-Output "Starting CosmosDB SetMinThroughput.ps1 script at $((Get-Date).ToString("yyyy-MM-dd HH:mm:ss"))"
write-output "Current Azure Subscription: $(az account show --query '[name, id]' -o json)"

# Only processing Kind = GlobalDocumentDB
$accounts = $null
$accounts = az cosmosdb list -g 'cosmicgbb-datalz' | ConvertFrom-Json | select-object -Property name, resourceGroup, id, kind | where-object {$_.kind -eq "GlobalDocumentDB"}

# For all Cosmos DB accounts
foreach($account in $accounts) {
    $accountName = $account.name
    Write-Output("`n`n`n*** Processing Cosmos DB: $accountName")

    $dbs = $null
    $dbs = az cosmosdb sql database list -g $account.resourceGroup -a $account.name | ConvertFrom-Json | select-object -Property name

    # For all databases
    foreach($db in $dbs) {
        Write-Output ("`n`n** Processing Database: $($db.name)")
        $dboffer = $null
        $dboffer = az cosmosdb sql database throughput show -g $account.resourceGroup -a $account.name -n $db.name | ConvertFrom-Json | select-object -ExpandProperty resource | select-object -Property minimumThroughput, throughput, autoscaleSettings
        
        if ($dboffer -ne $null) {
            Write-Output "Database Offer found. $($db.name) is a shared database throughput." 
            $autoscale = $null
            $autoscale = $dboffer | select-object -ExpandProperty autoscaleSettings

            if ($autoscale -ne $null) {
                Write-Output "Autoscale is enabled. Current max: $($autoscale.maxThroughput), suggested autoscale max throughput: $($dboffer.minimumThroughput)"
                if ($autoscale.maxThroughput -ne $dboffer.minimumThroughput) {
                    Write-Output "Setting autoscale max throughput to: $($dboffer.minimumThroughput)"
                    az cosmosdb sql database throughput update -g $account.resourceGroup -a $account.name -n $db.name --max-throughput $dboffer.minimumThroughput
                }
            }
            else {
                Write-Output "Autoscale is disabled. Current max: $($dboffer.throughput) `nSuggested shared db throughput minimum: $($dboffer.minimumThroughput)"
                if ($dboffer.throughput -ne $dboffer.minimumThroughput) {
                    Write-Output "Setting shared db throughput to: $($dboffer.minimumThroughput)"
                    az cosmosdb sql database throughput update -g $account.resourceGroup -a $account.name -n $db.name --throughput $dboffer.minimumThroughput
                }
            }
        }

        $colls = $null
        $colls = az cosmosdb sql container list -g $account.resourceGroup -a $account.name -d $db.name | ConvertFrom-Json | select-object -Property name

        # For all containers
        foreach($coll in $colls) {
            Write-Output ("`n* Processing Container: $($coll.name)")
            $colloffer = $null
            $colloffer = az cosmosdb sql container throughput show -g $account.resourceGroup -a $account.name -d $db.name -n $coll.name | ConvertFrom-Json | select-object -ExpandProperty resource | select-object -Property minimumThroughput, throughput, autoscaleSettings
            
            if ($colloffer -ne $null) {
                Write-Output "Container Offer found. $($coll.name) has throughput setting." 
                $autoscale = $null
                $autoscale = $colloffer | select-object -ExpandProperty autoscaleSettings

                if ($autoscale -ne $null) {
                    Write-Output "Autoscale is enabled. Current max: $($autoscale.maxThroughput) `nSuggested autoscale max throughput: $($colloffer.minimumThroughput)"
                    if ($autoscale.maxThroughput -ne $colloffer.minimumThroughput) {
                        Write-Output "Setting autoscale max throughput to: $($colloffer.minimumThroughput)"
                        az cosmosdb sql container throughput update -g $account.resourceGroup -a $account.name -d $db.name -n $coll.name --max-throughput $colloffer.minimumThroughput
                    }
                }
                else {
                    Write-Output "Autoscale is disabled. Current max: $($colloffer.throughput) `nSuggested shared container throughput minimum: $($colloffer.minimumThroughput)"
                    if ($colloffer.throughput -ne $colloffer.minimumThroughput) {
                        Write-Output "Setting shared container throughput to: $($colloffer.minimumThroughput)"
                        az cosmosdb sql container throughput update -g $account.resourceGroup -a $account.name -d $db.name -n $coll.name --throughput $colloffer.minimumThroughput
                    }
                }
            }
        }
    }
}

Stop-Transcript