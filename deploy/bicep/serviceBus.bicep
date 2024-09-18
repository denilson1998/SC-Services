param serviceBusName string

resource serviceBus 'Microsoft.ServiceBus/namespaces@2021-01-01-preview' = {
  name: serviceBusName
  location: resourceGroup().location
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
}

resource authorizationRulesERP 'Microsoft.ServiceBus/namespaces/AuthorizationRules@2021-01-01-preview' = {
  name: '${serviceBus.name}/ERP'
  properties: {
    rights: [
      'Listen'
      'Send'
    ]
  }
}

resource topicsRoleChange 'Microsoft.ServiceBus/namespaces/topics@2021-01-01-preview' = {
  name: '${serviceBus.name}/sbt-client-organization-role-changed'
  properties: {
    defaultMessageTimeToLive: 'P14D'
  }
}

resource topicsRoleEnable 'Microsoft.ServiceBus/namespaces/topics@2021-01-01-preview' = {
  name: '${serviceBus.name}/sbt-client-organization-role-enable'
  properties: {
    defaultMessageTimeToLive: 'P14D'
  }
}

resource topicsRoleRemoved 'Microsoft.ServiceBus/namespaces/topics@2021-01-01-preview' = {
  name: '${serviceBus.name}/sbt-client-organization-role-removed'
  properties: {
    defaultMessageTimeToLive: 'P14D'
  }
}

resource topicsClientRemoved 'Microsoft.ServiceBus/namespaces/topics@2021-01-01-preview' = {
  name: '${serviceBus.name}/sbt-client-removed'
  properties: {
    defaultMessageTimeToLive: 'P14D'
  }
}

var apisWithOrganizationRoleChangeSubscription = [
  'stocks'
  'sells'
  'delivery'
  'payments'
  'expenses'
]

resource subscriptionRoleChange 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2021-01-01-preview' = [for apiName in apisWithOrganizationRoleChangeSubscription: {
  name: '${topicsRoleChange.name}/sbs-${apiName}-synchronize-organization-role-change'
  properties: {
    lockDuration: 'PT30S'
    defaultMessageTimeToLive: 'P14D'
  }
}]

resource subscriptionRoleEnable 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2021-01-01-preview' = [for apiName in apisWithOrganizationRoleChangeSubscription: {
  name: '${topicsRoleEnable.name}/sbs-${apiName}-synchronize-organization-role-enable'
  properties: {
    lockDuration: 'PT30S'
    defaultMessageTimeToLive: 'P14D'
  }
}]

resource subscriptionRoleRemove 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2021-01-01-preview' = [for apiName in apisWithOrganizationRoleChangeSubscription: {
  name: '${topicsRoleRemoved.name}/sbs-${apiName}-synchronize-organization-role-remove'
  properties: {
    lockDuration: 'PT30S'
    defaultMessageTimeToLive: 'P14D'
  }
}]

resource subscriptionClientRemove 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2021-01-01-preview' = [for apiName in apisWithOrganizationRoleChangeSubscription: {
  name: '${topicsClientRemoved.name}/sbs-${apiName}-synchronize-client-remove'
  properties: {
    lockDuration: 'PT30S'
    defaultMessageTimeToLive: 'P14D'
  }
}]

resource topicsStockCreated 'Microsoft.ServiceBus/namespaces/topics@2021-01-01-preview' = {
  name: '${serviceBus.name}/sbt-stock-created'
  properties: {
    defaultMessageTimeToLive: 'P14D'
  }
}

resource topicsStockUpdated 'Microsoft.ServiceBus/namespaces/topics@2021-01-01-preview' = {
  name: '${serviceBus.name}/sbt-stock-updated'
  properties: {
    defaultMessageTimeToLive: 'P14D'
  }
}

resource topicsStockRemoved 'Microsoft.ServiceBus/namespaces/topics@2021-01-01-preview' = {
  name: '${serviceBus.name}/sbt-stock-removed'
  properties: {
    defaultMessageTimeToLive: 'P14D'
  }
}

var apisWithStockSubscription = [
  'sells'
]

resource subscriptionStockCreated 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2021-01-01-preview' = [for apiName in apisWithStockSubscription: {
  name: '${topicsStockCreated.name}/sbs-${apiName}-synchronize-stock-created'
  properties: {
    lockDuration: 'PT30S'
    defaultMessageTimeToLive: 'P14D'
  }
}]

resource subscriptionStockUpdated 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2021-01-01-preview' = [for apiName in apisWithStockSubscription: {
  name: '${topicsStockUpdated.name}/sbs-${apiName}-synchronize-stock-updated'
  properties: {
    lockDuration: 'PT30S'
    defaultMessageTimeToLive: 'P14D'
  }
}]

resource subscriptionStockRemoved 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2021-01-01-preview' = [for apiName in apisWithStockSubscription: {
  name: '${topicsStockRemoved.name}/sbs-${apiName}-synchronize-stock-removed'
  properties: {
    lockDuration: 'PT30S'
    defaultMessageTimeToLive: 'P14D'
  }
}]

resource topicsOrderCreated 'Microsoft.ServiceBus/namespaces/topics@2021-01-01-preview' = {
  name: '${serviceBus.name}/sbt-order-created'
  properties: {
    defaultMessageTimeToLive: 'P14D'
  }
}

resource topicsOrderRemoved 'Microsoft.ServiceBus/namespaces/topics@2021-01-01-preview' = {
  name: '${serviceBus.name}/sbt-order-removed'
  properties: {
    defaultMessageTimeToLive: 'P14D'
  }
}

var apisWithOrderSubscription = [
  'stocks'
  'payments'
]

resource subscriptionOrderCreated 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2021-01-01-preview' = [for apiName in apisWithOrderSubscription: {
  name: '${topicsOrderCreated.name}/sbs-${apiName}-synchronize-order-created'
  properties: {
    lockDuration: 'PT30S'
    defaultMessageTimeToLive: 'P14D'
  }
}]

resource subscriptionOrderRemoved 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2021-01-01-preview' = [for apiName in apisWithOrderSubscription: {
  name: '${topicsOrderRemoved.name}/sbs-${apiName}-synchronize-order-removed'
  properties: {
    lockDuration: 'PT30S'
    defaultMessageTimeToLive: 'P14D'
  }
}]

resource topicsBillPaid 'Microsoft.ServiceBus/namespaces/topics@2021-01-01-preview' = {
  name: '${serviceBus.name}/sbt-bill-paid'
  properties: {
    defaultMessageTimeToLive: 'P14D'
  }
}

var apisWithBillPaidSubscription = [
  'sells'
]

resource subscriptionBillPaid 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2021-01-01-preview' = [for apiName in apisWithBillPaidSubscription: {
  name: '${topicsBillPaid.name}/sbs-${apiName}-synchronize-bill-paid'
  properties: {
    lockDuration: 'PT30S'
    defaultMessageTimeToLive: 'P14D'
  }
}]

output ConnectionStrings__AzureServiceBus string = listKeys(authorizationRulesERP.id, authorizationRulesERP.apiVersion).primaryConnectionString
