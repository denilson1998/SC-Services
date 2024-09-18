
param($sqlAdministratorPassword, $azureAdSecret, $templateFile, $resourceGroupName,$paramfile)

$location = 'eastus'

"Printing azure cli version..."
az --version

Write-Output "Creating resource group $resourceGroupName in location $location...";
az group create --name $resourceGroupName --location $location

"Initializing deployment"
az deployment group create `
    -n 'main-deployment' `
    -g $resourceGroupName `
    -f $templateFile `
    -p $paramfile sqlAdministratorPassword=$sqlAdministratorPassword azureAdSecret=$azureAdSecret  --verbose