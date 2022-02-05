# https://theitbros.com/install-azure-powershell/
#
# https://docs.microsoft.com/en-us/powershell/module/az.accounts/connect-azaccount?view=azps-7.1.0
# 
# Connect-AzAccount -Tenant "--tenantId--"
# AZ LOGIN --tenant "--tenantId--"
#

$tenantId = "--tenantId--"
$gpAdmins = "demo-admins"
$gpUsers = "demo-users"

function testParams {

	if (!$tenantId) 
	{ 
		Write-Host "tenantId is null"
		exit 1
	}
}

testParams

function CreateGroup([string]$name) {
    Write-Host " - Create new group"
    $group = az ad group create --display-name $name --mail-nickname $name

    $gpObjectId = ($group | ConvertFrom-Json).objectId
    Write-Host " $gpObjectId $name"
}

Write-Host "Creating groups"

##################################
### Create groups
##################################

CreateGroup $gpAdmins
CreateGroup $gpUsers

#az ad group list --display-name $groupName

return
