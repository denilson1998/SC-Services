param name string

param administratorLogin string

@secure()
param administratorPassword string

param environment string

param databases array

var databaseArray = [for database in databases: '${toLower(database)}']

resource sqlServer 'Microsoft.Sql/servers@2021-02-01-preview' = {
  name: 'sql-${name}-${environment}' 
  location: resourceGroup().location
  properties: {
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorPassword
  }
}

resource allowAllWindowsAzureIps 'Microsoft.Sql/servers/firewallRules@2021-02-01-preview' = {
  parent: sqlServer
  name: 'AllowAllAzureIps'
  properties: {
    endIpAddress: '0.0.0.0'
    startIpAddress: '0.0.0.0'
  }
}

resource sqlDb 'Microsoft.Sql/servers/databases@2021-02-01-preview' = [for database in databaseArray: {
  name: '${sqlServer.name}/sqldb-${name}-${database}-${environment}'
  location: resourceGroup().location
  sku: {
    name: 'Basic'
    tier: 'Basic'
  }
}]

output sqlServerFullyQualifiedDomainName string =sqlServer.properties.fullyQualifiedDomainName
// output databaseName array = [for (name,i) in databases: {
//   name: sqlDb[i].name
// }]

