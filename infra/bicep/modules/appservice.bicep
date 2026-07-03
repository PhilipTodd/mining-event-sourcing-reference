param location string
param appServicePlanName string
param apiAppName string
param uiAppName string
param workerAppName string
param applicationInsightsConnectionString string

param cosmosDatabaseName string
param cosmosContainerName string

@secure()
param cosmosConnectionString string

@secure()
param sqlConnectionString string

@secure()
param serviceBusConnectionString string

param serviceBusTopicName string
param serviceBusSubscriptionName string

param entraTenantId string
param entraApiClientId string

param skuName string = 'B1'
param skuTier string = 'Basic'

resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: skuName
    tier: skuTier
  }
  properties: {
    reserved: false
  }
}

resource apiApp 'Microsoft.Web/sites@2023-12-01' = {
  name: apiAppName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      alwaysOn: true
      minTlsVersion: '1.2'
        appSettings: [
          {
            name: 'ASPNETCORE_ENVIRONMENT'
            value: 'Development'
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

          {
            name: 'Cors__AllowedOrigins__0'
            value: 'http://localhost:4200'
          }
          {
            name: 'Cors__AllowedOrigins__1'
            value: 'https://demo.event-sourcing.ausdatatech.com.au'
          }
        ]
    }
  }
}

resource uiApp 'Microsoft.Web/sites@2023-12-01' = {
  name: uiAppName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      alwaysOn: true
      minTlsVersion: '1.2'
      appSettings: [
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
        {
          name: 'SCM_DO_BUILD_DURING_DEPLOYMENT'
          value: 'false'
        }
        {
          name: 'WEBSITE_NODE_DEFAULT_VERSION'
          value: '~22'
        }
      ]
    }
  }
}

resource workerApp 'Microsoft.Web/sites@2023-12-01' = {
  name: workerAppName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      alwaysOn: true
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Development'
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: applicationInsightsConnectionString
        }
      ]
    }
  }
}

output appServicePlanName string = appServicePlan.name
output apiAppName string = apiApp.name
output uiAppName string = uiApp.name
output workerAppName string = workerApp.name
output apiAppDefaultHostName string = apiApp.properties.defaultHostName
output workerAppDefaultHostName string = workerApp.properties.defaultHostName