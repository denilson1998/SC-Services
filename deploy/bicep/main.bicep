param sqlAdministratorLogin string

@secure()
param sqlAdministratorPassword string

@allowed([
  'dev'
  'prod'
])
param env string

param name string

param appServiceSku string

param storageAccountName string

@allowed([
  'Standard_LRS'
  'Standard_GRS'
  'Standard_ZRS'
  'Premium_LRS'
])
param storageAccountSku string

param storageAccountAccessTier string

param IdentityProvider__BaseUrl string

param IdentityProvider__Audience string

param Twilio__AccountSid string

param Twilio__AuthToken string

param Twilio__VerificationSid string

param Auth0__ApiManagement__ClientId string

param Auth0__ApiManagement__ClientSecret string

param Auth0__AuthenticationApi__ClientId string

param Auth0__AuthenticationApi__ClientSecret string

param Auth0__AuthenticationApi__Audience string

param Auth0__BaseUrl string

param Auth0__SwaggerClientId string

param Auth0__SwaggerClientSecret string

param Auth0__SwaggerAppName string

param Bouncer__ClientId string

param Bouncer__ClientSecret string

param Bouncer__Audience string

param Bouncer__BaseUrl string

param CourierConfig__BaseUrl string

param ConsentServerApiConfig__BaseUrl string

param QRServiceConfig__BaseUrl string

param OnboardingApiConfig__BaseUrl string
param SellsApiConfig__BaseUrl string
param StockApiConfig__BaseUrl string
param PaymentsApiConfig__BaseUrl string
param serviceBusName string

param appiWorkspaceGroupName string

param appiWorkspaceName string

param Mandrill__ApiKey string

param Mandrill__From string

param Mandrill__To string


@secure()
param CredentialsBasic__PasswordPlainText string

param AzureAD__BaseUrl string

param AzureAD__ClientId string

param AzureAD__TenantId string

@secure()
param azureAdSecret string

param AzureAD__Scope string

param GoogleApplicationCredentials string

@secure()
param Auth0ApiConfig__ClientSecret string

param Auth0ApiConfig__BaseUrl string

param Auth0ApiConfig__ClientId string

param Auth0ApiConfig__Audience string


param appiKind string
var databases = [
  'Delivery'
  'Payments'
]

module insights 'appInsights.bicep' = {
  name: 'appi-deploy'
  params:{
    name: name
    kind: appiKind
    workspaceGroupName: appiWorkspaceGroupName
    workspaceName: appiWorkspaceName
    environment: env
  }
}

module serviceBus 'serviceBus.bicep' = {
  name: 'service-bus-deploy'
  params: {
    serviceBusName: serviceBusName
  }
}

module sql 'sql.bicep' = {
  name: 'sql-deploy'
  params: {
    name: name
    databases: databases
    administratorLogin: sqlAdministratorLogin
    administratorPassword: sqlAdministratorPassword
    environment: env
  }
}
