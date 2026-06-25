targetScope = 'resourceGroup'

param location string = resourceGroup().location

param environmentName string
param projectName string

param cosmosAccountName string
param cosmosDatabaseName string
param cosmosContainerName string
param cosmosThroughput int = 400

param sqlServerName string
param sqlDatabaseName string
param sqlAdministratorLogin string

param logAnalyticsWorkspaceName string
param applicationInsightsName string

@secure()
param sqlAdministratorLoginPassword string

param serviceBusNamespaceName string
param serviceBusTopicName string = 'domain-events'
param serviceBusSubscriptionName string = 'blast-plan-projections'

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

module sql '../../modules/sql.bicep' = {
  name: 'sql-${projectName}-${environmentName}'
  params: {
    location: location
    sqlServerName: sqlServerName
    sqlDatabaseName: sqlDatabaseName
    administratorLogin: sqlAdministratorLogin
    administratorLoginPassword: sqlAdministratorLoginPassword
  }
}

module serviceBus '../../modules/servicebus.bicep' = {
  name: 'servicebus-${projectName}-${environmentName}'
  params: {
    location: location
    namespaceName: serviceBusNamespaceName
    topicName: serviceBusTopicName
    subscriptionName: serviceBusSubscriptionName
  }
}

module monitoring '../../modules/applicationinsights.bicep' = {
  name: 'monitoring-${projectName}-${environmentName}'
  params: {
    location: location
    logAnalyticsWorkspaceName: logAnalyticsWorkspaceName
    applicationInsightsName: applicationInsightsName
  }
}

output cosmosAccountName string = cosmos.outputs.accountName
output cosmosDatabaseName string = cosmos.outputs.databaseName
output cosmosContainerName string = cosmos.outputs.containerName

output sqlServerName string = sql.outputs.sqlServerName
output sqlDatabaseName string = sql.outputs.sqlDatabaseName
output sqlServerFullyQualifiedDomainName string = sql.outputs.sqlServerFullyQualifiedDomainName

output serviceBusNamespaceName string = serviceBus.outputs.namespaceName
output serviceBusTopicName string = serviceBus.outputs.topicName
output serviceBusSubscriptionName string = serviceBus.outputs.subscriptionName

output logAnalyticsWorkspaceName string = monitoring.outputs.logAnalyticsWorkspaceName
output applicationInsightsName string = monitoring.outputs.applicationInsightsName
output applicationInsightsConnectionString string = monitoring.outputs.applicationInsightsConnectionString

