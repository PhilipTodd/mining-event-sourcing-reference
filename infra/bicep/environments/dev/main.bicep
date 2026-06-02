targetScope = 'resourceGroup'

param location string = resourceGroup().location
param environmentName string
param projectName string
param cosmosAccountName string
param cosmosDatabaseName string
param cosmosContainerName string
param cosmosThroughput int = 400

module cosmos '../../modules/cosmosdb.bicep' = {
  name: 'cosmos-${projectName}-${environmentName}'
  params: {
    location: location
    accountName: cosmosAccountName
    databaseName: cosmosDatabaseName
    containerName: cosmosContainerName
    throughput: cosmosThroughput
  }
}

output cosmosAccountName string = cosmos.outputs.accountName
output cosmosDatabaseName string = cosmos.outputs.databaseName
output cosmosContainerName string = cosmos.outputs.containerName