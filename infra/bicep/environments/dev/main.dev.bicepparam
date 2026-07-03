using './main.bicep'

param location = 'australiaeast'
param environmentName = 'dev'
param projectName = 'esr'

param cosmosAccountName = 'esr-dev-cosmos-events'
param cosmosDatabaseName = 'blast-planning'
param cosmosContainerName = 'events'
param cosmosThroughput = 400

param sqlServerName = 'esr-dev-sql-readmodels'
param sqlDatabaseName = 'BlastPlanningReadModels'
param sqlAdministratorLogin = 'sqladmin'
param sqlAdministratorLoginPassword = readEnvironmentVariable('SQL_ADMIN_PASSWORD')

param serviceBusNamespaceName = 'esr-dev-servicebus'
param serviceBusTopicName = 'domain-events'
param serviceBusSubscriptionName = 'blast-plan-projections'

param logAnalyticsWorkspaceName = 'esr-dev-log'
param applicationInsightsName = 'esr-dev-appi'

param appServicePlanName = 'esr-dev-plan'
param apiAppName = 'esr-dev-api'
param uiAppName = 'esr-dev-ui'
param workerAppName = 'esr-dev-worker'

param entraTenantId = 'f4fcd45c-104f-4dd3-b7bf-3475e83ce097'
param entraApiClientId = '9b84c3bc-479f-4f57-b5eb-8efef1f6e062'

param cosmosConnectionString = readEnvironmentVariable('COSMOS_CONNECTION_STRING')
param sqlConnectionString = readEnvironmentVariable('SQL_CONNECTION_STRING')
param serviceBusConnectionString = readEnvironmentVariable('SERVICEBUS_CONNECTION_STRING')
