param location string
param namespaceName string
param topicName string = 'domain-events'
param subscriptionName string = 'blast-plan-projections'

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2024-01-01' = {
  name: namespaceName
  location: location
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
  properties: {
    minimumTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
  }
}

resource topic 'Microsoft.ServiceBus/namespaces/topics@2024-01-01' = {
  parent: serviceBusNamespace
  name: topicName
  properties: {
    defaultMessageTimeToLive: 'P14D'
    duplicateDetectionHistoryTimeWindow: 'PT10M'
    enableBatchedOperations: true
    enablePartitioning: false
    maxMessageSizeInKilobytes: 256
    requiresDuplicateDetection: false
  }
}

resource subscription 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2024-01-01' = {
  parent: topic
  name: subscriptionName
  properties: {
    lockDuration: 'PT1M'
    maxDeliveryCount: 10
    defaultMessageTimeToLive: 'P14D'
    deadLetteringOnMessageExpiration: true
  }
}

output namespaceName string = serviceBusNamespace.name
output topicName string = topic.name
output subscriptionName string = subscription.name