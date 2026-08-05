using './main.bicep'

// ============================================================================
// Environment
// ============================================================================

param environment = 'dev'

// ============================================================================
// Resource Groups
// ============================================================================

param applicationResourceGroupName = 'rg-event-sourcing-dev'
param platformResourceGroupName = 'rg-platform-dev'

// ============================================================================
// Locations
// ============================================================================

param applicationLocation = 'australiaeast'

// ============================================================================
// Shared Platform Resources
// ============================================================================

param appServicePlanName = 'asp-platform-dev'

param applicationInsightsName = 'appi-platform-dev'

param sqlServerName = 'sql-adt-platform-dev'
param sqlDatabaseName = 'ReferenceProjectsDb'

param cosmosAccountName = 'cosmos-adt-platform-dev'
param cosmosDatabaseName = 'ReferenceProjects'

param serviceBusNamespaceName = 'sb-adt-platform-dev'

// ============================================================================
// Application Resources
// ============================================================================

param apiAppName = 'api-adt-blastplanning-dev'

param uiAppName = 'web-adt-blastplanning-dev'

param workerAppName = 'worker-adt-blastplanning-dev'

// ============================================================================
// Cosmos DB
// ============================================================================

param cosmosContainerName = 'blastplanning-events'

param cosmosPartitionKeyPath = '/streamId'

// ============================================================================
// Service Bus
// ============================================================================

param serviceBusTopicName = 'domain-events'

param serviceBusSubscriptionName = 'blast-plan-projections'

// ============================================================================
// Microsoft Entra ID
// ============================================================================

param entraTenantId = 'f4fcd45c-104f-4dd3-b7bf-3475e83ce097'

param entraApiClientId = '9b84c3bc-479f-4f57-b5eb-8efef1f6e062'

// ============================================================================
// API CORS
// ============================================================================

param corsAllowedOrigins = [
  'http://localhost:4200'
  'https://demo.event-sourcing.ausdatatech.com.au'
]

// ============================================================================
// Connection Strings
//
// Values supplied by GitHub Environment secrets.
// ============================================================================

param cosmosConnectionString = readEnvironmentVariable('COSMOS_CONNECTION_STRING')

param sqlConnectionString = readEnvironmentVariable('SQL_CONNECTION_STRING')

param serviceBusConnectionString = readEnvironmentVariable('SERVICEBUS_CONNECTION_STRING')