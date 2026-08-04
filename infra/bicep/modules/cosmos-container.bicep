targetScope = 'resourceGroup'

@description('Name of the existing Azure Cosmos DB account.')
param accountName string

@description('Name of the existing Cosmos DB for NoSQL database.')
param databaseName string

@description('Name of the Cosmos DB container.')
param containerName string

@description('Partition-key path used by the container.')
param partitionKeyPath string = '/streamId'

@description('Partition-key version.')
@allowed([
  1
  2
])
param partitionKeyVersion int = 2

@description('Indexing mode used by the container.')
@allowed([
  'consistent'
  'none'
])
param indexingMode string = 'consistent'

resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2025-10-15' existing = {
  name: accountName
}

resource cosmosDatabase 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2025-10-15' existing = {
  parent: cosmosAccount
  name: databaseName
}

resource cosmosContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2025-10-15' = {
  parent: cosmosDatabase
  name: containerName

  properties: {
    resource: {
      id: containerName

      partitionKey: {
        paths: [
          partitionKeyPath
        ]
        kind: 'Hash'
        version: partitionKeyVersion
      }

      indexingPolicy: {
        indexingMode: indexingMode
        automatic: true

        includedPaths: [
          {
            path: '/*'
          }
        ]

        excludedPaths: [
          {
            path: '/"_etag"/?'
          }
        ]
      }
    }

    options: {}
  }
}

output containerId string = cosmosContainer.id
output containerName string = cosmosContainer.name