targetScope = 'subscription'

// ============================================================================
// Deployment
// ============================================================================

@description('Deployment environment.')
@allowed([
  'dev'
  'test'
  'prod'
])
param environment string = 'dev'

@description('Location of the Event Sourcing application resources.')
param applicationLocation string

@description('Name of the Event Sourcing application resource group.')
param applicationResourceGroupName string

@description('Tags applied to application-owned resources.')
param tags object = {
  environment: environment
  managedBy: 'bicep'
  project: 'mining-event-sourcing-reference'
  capability: 'blastplanning'
}

// ============================================================================
// Shared platform resources
// ============================================================================

@description('Name of the shared platform resource group.')
param platformResourceGroupName string

@description('Name of the shared Linux App Service Plan.')
param appServicePlanName string

@description('Name of the shared Application Insights resource.')
param applicationInsightsName string

@description('Name of the shared Azure SQL logical server.')
param sqlServerName string

@description('Name of the shared Azure SQL database.')
param sqlDatabaseName string

@description('Name of the shared Cosmos DB account.')
param cosmosAccountName string

@description('Name of the shared Cosmos DB database.')
param cosmosDatabaseName string

@description('Name of the shared Service Bus namespace.')
param serviceBusNamespaceName string

// ============================================================================
// Application compute
// ============================================================================

@description('Globally unique name of the Blast Planning API App Service.')
param apiAppName string

@description('Globally unique name of the Blast Planning UI App Service.')
param uiAppName string

@description('Globally unique name of the App Service hosting the Projection WebJob.')
param workerAppName string

// ============================================================================
// Cosmos DB
// ============================================================================

@description('Name of the Blast Planning event-store container.')
param cosmosContainerName string

@description('Partition-key path for the event-store container.')
param cosmosPartitionKeyPath string = '/streamId'

// ============================================================================
// Service Bus
// ============================================================================

@description('Name of the Blast Planning domain-events topic.')
param serviceBusTopicName string

@description('Name of the Blast Planning projection subscription.')
param serviceBusSubscriptionName string

// ============================================================================
// Microsoft Entra ID and CORS
// ============================================================================

@description('Microsoft Entra tenant ID.')
param entraTenantId string

@description('Client ID of the Blast Planning API app registration.')
param entraApiClientId string

@description('Allowed browser origins for the API.')
param corsAllowedOrigins array

// ============================================================================
// Connection strings
//
// Retained during this minimal refactor. Managed identity migration is outside
// the scope of the current infrastructure relocation.
// ============================================================================

@secure()
@description('Connection string for the shared Cosmos DB account.')
param cosmosConnectionString string

@secure()
@description('Connection string for ReferenceProjectsDb.')
param sqlConnectionString string

@secure()
@description('Connection string for the shared Service Bus namespace.')
param serviceBusConnectionString string

// ============================================================================
// Application resource group
// ============================================================================

resource applicationResourceGroup 'Microsoft.Resources/resourceGroups@2024-03-01' = {
  name: applicationResourceGroupName
  location: applicationLocation
  tags: tags
}

// ============================================================================
// Shared platform resources
// ============================================================================

resource sharedAppServicePlan 'Microsoft.Web/serverfarms@2024-04-01' existing = {
  name: appServicePlanName
  scope: resourceGroup(platformResourceGroupName)
}

resource sharedApplicationInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: applicationInsightsName
  scope: resourceGroup(platformResourceGroupName)
}

resource sharedSqlServer 'Microsoft.Sql/servers@2023-08-01' existing = {
  name: sqlServerName
  scope: resourceGroup(platformResourceGroupName)
}

resource sharedSqlDatabase 'Microsoft.Sql/servers/databases@2023-08-01' existing = {
  parent: sharedSqlServer
  name: sqlDatabaseName
}

resource sharedCosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2025-10-15' existing = {
  name: cosmosAccountName
  scope: resourceGroup(platformResourceGroupName)
}

resource sharedCosmosDatabase 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2025-10-15' existing = {
  parent: sharedCosmosAccount
  name: cosmosDatabaseName
}

resource sharedServiceBusNamespace 'Microsoft.ServiceBus/namespaces@2026-01-01' existing = {
  name: serviceBusNamespaceName
  scope: resourceGroup(platformResourceGroupName)
}

// ============================================================================
// Application-owned Cosmos container
// ============================================================================

module cosmosContainer '../../modules/cosmos-container.bicep' = {
  name: 'blastplanning-cosmos-container'
  scope: resourceGroup(platformResourceGroupName)

  params: {
    accountName: sharedCosmosAccount.name
    databaseName: sharedCosmosDatabase.name
    containerName: cosmosContainerName
    partitionKeyPath: cosmosPartitionKeyPath
  }
}

// ============================================================================
// Application-owned Service Bus topic and subscription
// ============================================================================

module serviceBus '../../modules/servicebus.bicep' = {
  name: 'blastplanning-servicebus'
  scope: resourceGroup(platformResourceGroupName)

  params: {
    namespaceName: sharedServiceBusNamespace.name
    topicName: serviceBusTopicName
    subscriptionName: serviceBusSubscriptionName
  }
}

// ============================================================================
// Application App Services
// ============================================================================

module appService '../../modules/appservice.bicep' = {
  name: 'blastplanning-appservices'
  scope: applicationResourceGroup

  params: {
    location: applicationLocation

    appServicePlanId: sharedAppServicePlan.id

    apiAppName: apiAppName
    uiAppName: uiAppName
    workerAppName: workerAppName

    applicationInsightsConnectionString: sharedApplicationInsights.properties.ConnectionString

    cosmosConnectionString: cosmosConnectionString
    cosmosDatabaseName: sharedCosmosDatabase.name
    cosmosContainerName: cosmosContainer.outputs.containerName

    sqlConnectionString: sqlConnectionString

    serviceBusConnectionString: serviceBusConnectionString
    serviceBusTopicName: serviceBus.outputs.topicName
    serviceBusSubscriptionName: serviceBus.outputs.subscriptionName

    entraTenantId: entraTenantId
    entraApiClientId: entraApiClientId

    corsAllowedOrigins: corsAllowedOrigins

    tags: tags
  }
}

// ============================================================================
// Outputs
// ============================================================================

output applicationResourceGroupName string = applicationResourceGroup.name
output applicationResourceGroupId string = applicationResourceGroup.id

output sharedAppServicePlanName string = sharedAppServicePlan.name
output sharedAppServicePlanId string = sharedAppServicePlan.id

output sharedSqlServerName string = sharedSqlServer.name
output sharedSqlDatabaseName string = sharedSqlDatabase.name

output sharedCosmosAccountName string = sharedCosmosAccount.name
output sharedCosmosDatabaseName string = sharedCosmosDatabase.name
output cosmosContainerName string = cosmosContainer.outputs.containerName

output sharedServiceBusNamespaceName string = sharedServiceBusNamespace.name
output serviceBusTopicName string = serviceBus.outputs.topicName
output serviceBusSubscriptionName string = serviceBus.outputs.subscriptionName

output apiAppName string = appService.outputs.apiAppName
output apiAppDefaultHostName string = appService.outputs.apiAppDefaultHostName

output uiAppName string = appService.outputs.uiAppName
output uiAppDefaultHostName string = appService.outputs.uiAppDefaultHostName

output workerAppName string = appService.outputs.workerAppName
output workerAppDefaultHostName string = appService.outputs.workerAppDefaultHostName