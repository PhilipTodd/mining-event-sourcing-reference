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

