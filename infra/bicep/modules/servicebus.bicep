targetScope = 'resourceGroup'

@description('Name of the existing Azure Service Bus namespace.')
param namespaceName string

@description('Name of the Service Bus topic used for Blast Planning domain events.')
param topicName string

@description('Name of the subscription used by the Blast Planning projection worker.')
param subscriptionName string

@description('Default time-to-live for messages published to the topic.')
param defaultMessageTimeToLive string = 'P14D'

@description('Duplicate-detection history window.')
param duplicateDetectionHistoryTimeWindow string = 'PT10M'

@description('Maximum subscription delivery count before a message is dead-lettered.')
@minValue(1)
param maxDeliveryCount int = 10

@description('Subscription message lock duration.')
param lockDuration string = 'PT1M'

@description('Subscription auto-delete-on-idle period. Empty means disabled.')
param autoDeleteOnIdle string = ''

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2024-01-01' existing = {
  name: namespaceName
}

resource topic 'Microsoft.ServiceBus/namespaces/topics@2024-01-01' = {
  parent: serviceBusNamespace
  name: topicName

  properties: {
    defaultMessageTimeToLive: defaultMessageTimeToLive

    requiresDuplicateDetection: true
    duplicateDetectionHistoryTimeWindow: duplicateDetectionHistoryTimeWindow

    enablePartitioning: false
    supportOrdering: true

    maxSizeInMegabytes: 1024
    status: 'Active'
  }
}

resource subscription 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2024-01-01' = {
  parent: topic
  name: subscriptionName

  properties: {
    lockDuration: lockDuration
    maxDeliveryCount: maxDeliveryCount

    deadLetteringOnMessageExpiration: true
    deadLetteringOnFilterEvaluationExceptions: true

    defaultMessageTimeToLive: defaultMessageTimeToLive
    status: 'Active'

    autoDeleteOnIdle: empty(autoDeleteOnIdle)
      ? null
      : autoDeleteOnIdle
  }
}

output topicId string = topic.id
output topicName string = topic.name

output subscriptionId string = subscription.id
output subscriptionName string = subscription.name