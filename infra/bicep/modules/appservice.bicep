targetScope = 'resourceGroup'

// ============================================================================
// Shared configuration
// ============================================================================

@description('Azure region for the App Services.')
param location string

@description('Resource ID of the existing shared Linux App Service Plan.')
param appServicePlanId string

@description('Tags applied to the App Services.')
param tags object = {}

// ============================================================================
// App Service names
// ============================================================================

@description('Globally unique name of the Blast Planning API App Service.')
param apiAppName string

@description('Globally unique name of the Blast Planning UI App Service.')
param uiAppName string

// ============================================================================
// Function apps
// ============================================================================

param functionAppName string

@secure()
param functionStorageConnectionString string

// ============================================================================
// Monitoring
// ============================================================================

@description('Application Insights connection string.')
param applicationInsightsConnectionString string

// ============================================================================
// Cosmos DB
// ============================================================================

@secure()
@description('Cosmos DB account connection string.')
param cosmosConnectionString string

@description('Cosmos DB database name.')
param cosmosDatabaseName string

@description('Cosmos DB event-store container name.')
param cosmosContainerName string

// ============================================================================
// Azure SQL
// ============================================================================

@secure()
@description('Azure SQL connection string for ReferenceProjectsDb.')
param sqlConnectionString string

// ============================================================================
// Service Bus
// ============================================================================

@secure()
@description('Azure Service Bus namespace connection string.')
param serviceBusConnectionString string

@description('Service Bus topic name.')
param serviceBusTopicName string

@description('Service Bus subscription name.')
param serviceBusSubscriptionName string

// ============================================================================
// Microsoft Entra ID
// ============================================================================

@description('Microsoft Entra tenant ID.')
param entraTenantId string

@description('Client ID of the Blast Planning API app registration.')
param entraApiClientId string

// ============================================================================
// CORS
// ============================================================================

@description('Origins permitted to call the Blast Planning API.')
param corsAllowedOrigins array

// ============================================================================
// Runtime configuration
// ============================================================================

@description('ASP.NET Core environment name.')
param aspNetCoreEnvironment string = 'Development'

@description('Linux runtime used by the ASP.NET Core API.')
param apiDotnetLinuxFxVersion string = 'DOTNETCORE|10.0'

@description('Linux runtime used by the isolated Azure Function.')
param functionDotnetLinuxFxVersion string = 'DOTNET-ISOLATED|10.0'

@description('Linux runtime used by the Angular UI host.')
param nodeLinuxFxVersion string = 'NODE|22-lts'

// Convert the CORS array into ASP.NET Core hierarchical app settings:
//
// Cors__AllowedOrigins__0
// Cors__AllowedOrigins__1
// ...
var corsAppSettings = [
  for (origin, index) in corsAllowedOrigins: {
    name: 'Cors__AllowedOrigins__${index}'
    value: origin
  }
]

var commonBackendAppSettings = [
  {
    name: 'ASPNETCORE_ENVIRONMENT'
    value: aspNetCoreEnvironment
  }
  {
    name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
    value: applicationInsightsConnectionString
  }
  {
    name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
    value: '~3'
  }

  {
    name: 'UseInMemoryEventStore'
    value: 'false'
  }
  {
    name: 'UseInMemoryReadModels'
    value: 'false'
  }

  {
    name: 'CosmosEventStore__ConnectionString'
    value: cosmosConnectionString
  }
  {
    name: 'CosmosEventStore__DatabaseName'
    value: cosmosDatabaseName
  }
  {
    name: 'CosmosEventStore__ContainerName'
    value: cosmosContainerName
  }

  {
    name: 'Sql__ConnectionString'
    value: sqlConnectionString
  }

  {
    name: 'ServiceBus__ConnectionString'
    value: serviceBusConnectionString
  }
  {
    name: 'ServiceBus__TopicName'
    value: serviceBusTopicName
  }
  {
    name: 'ServiceBus__SubscriptionName'
    value: serviceBusSubscriptionName
  }
]

// ============================================================================
// Blast Planning API
// ============================================================================

resource apiApp 'Microsoft.Web/sites@2024-04-01' = {
  name: apiAppName
  location: location
  kind: 'app,linux'
  tags: tags

  properties: {
    serverFarmId: appServicePlanId
    httpsOnly: true

    siteConfig: {
      linuxFxVersion: apiDotnetLinuxFxVersion
      alwaysOn: true

      minTlsVersion: '1.2'
      ftpsState: 'Disabled'
      http20Enabled: true
      webSocketsEnabled: false

      appSettings: concat(
        commonBackendAppSettings,
        [
          {
            name: 'AzureAd__Instance'
            value: 'https://login.microsoftonline.com/'
          }
          {
            name: 'AzureAd__TenantId'
            value: entraTenantId
          }
          {
            name: 'AzureAd__ClientId'
            value: entraApiClientId
          }
          {
            name: 'AzureAd__Audience'
            value: 'api://${entraApiClientId}'
          }
        ],
        corsAppSettings
      )
    }
  }
}

// ============================================================================
// Angular UI
//
// The Angular build output is deployed to /home/site/wwwroot. PM2 serves the
// static files and redirects client-side routes to index.html.
// ============================================================================

resource uiApp 'Microsoft.Web/sites@2024-04-01' = {
  name: uiAppName
  location: location
  kind: 'app,linux'
  tags: tags

  properties: {
    serverFarmId: appServicePlanId
    httpsOnly: true

    siteConfig: {
      linuxFxVersion: nodeLinuxFxVersion
      alwaysOn: true

      minTlsVersion: '1.2'
      ftpsState: 'Disabled'
      http20Enabled: true

      appCommandLine: 'pm2 serve /home/site/wwwroot --no-daemon --spa'

      appSettings: [
        {
          name: 'SCM_DO_BUILD_DURING_DEPLOYMENT'
          value: 'false'
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: applicationInsightsConnectionString
        }
      ]
    }
  }
}

// ============================================================================
// Projection Function app
//
// This Function app is repossible for projections. It is hosted within an App Service Plan.
// The application deployment pipeline remains responsible for deploying the
// Function app.
// ============================================================================

resource projectionFunctionApp 'Microsoft.Web/sites@2024-04-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp,linux'
  tags: tags

  properties: {
    serverFarmId: appServicePlanId
    httpsOnly: true

    siteConfig: {
      linuxFxVersion: functionDotnetLinuxFxVersion
      alwaysOn: true
      minTlsVersion: '1.2'
      ftpsState: 'Disabled'
      http20Enabled: true

      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: functionStorageConnectionString
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: applicationInsightsConnectionString
        }
        {
          name: 'UseInMemoryEventStore'
          value: 'false'
        }
        {
          name: 'UseInMemoryReadModels'
          value: 'false'
        }
        {
          name: 'Sql__ConnectionString'
          value: sqlConnectionString
        }
        {
          name: 'ServiceBus__ConnectionString'
          value: serviceBusConnectionString
        }
        {
          name: 'ServiceBus__TopicName'
          value: serviceBusTopicName
        }
        {
          name: 'ServiceBus__SubscriptionName'
          value: serviceBusSubscriptionName
        }
      ]
    }
  }
}

// ============================================================================
// Outputs
// ============================================================================

output apiAppId string = apiApp.id
output apiAppName string = apiApp.name
output apiAppDefaultHostName string = apiApp.properties.defaultHostName

output uiAppId string = uiApp.id
output uiAppName string = uiApp.name
output uiAppDefaultHostName string = uiApp.properties.defaultHostName

output functionAppId string = projectionFunctionApp.id
output functionAppName string = projectionFunctionApp.name
output functionAppDefaultHostName string = projectionFunctionApp.properties.defaultHostName
