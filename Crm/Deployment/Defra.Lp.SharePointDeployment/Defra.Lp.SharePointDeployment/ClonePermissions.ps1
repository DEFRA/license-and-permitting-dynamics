##Variables for Processing 
Param(
	[string]$SiteUrl,
	[string]$SiteUrlDest,
	[string]$UserName,
	[string]$Password,
	[string]$UserNameDest,
	[string]$PasswordDest
)

Add-Type -Path ".\packages\Microsoft.SharePointOnline.CSOM.16.1.7723.1200\lib\net45\Microsoft.SharePoint.Client.dll"   
Add-Type -Path ".\packages\Microsoft.SharePointOnline.CSOM.16.1.7723.1200\lib\net45\Microsoft.SharePoint.Client.Runtime.dll"  

Write-Host "Log in to source SharePoint site " $SiteUrl

$Credentials = New-Object Microsoft.SharePoint.Client.SharePointOnlineCredentials($UserName,(ConvertTo-SecureString $Password -AsPlainText -Force)) 
$Context = New-Object Microsoft.SharePoint.Client.ClientContext($SiteUrl) 
$Context.Credentials = $credentials 
$Context.ExecuteQuery()

Write-Host "Log in to source SharePoint site " $SiteUrl " successfull"

Write-Host "Log in to destination SharePoint site " $SiteUrlDest

$CredentialsDest = New-Object Microsoft.SharePoint.Client.SharePointOnlineCredentials($UserNameDest,(ConvertTo-SecureString $PasswordDest -AsPlainText -Force)) 
$ContextDest = New-Object Microsoft.SharePoint.Client.ClientContext($SiteUrlDest) 
$ContextDest.Credentials = $CredentialsDest 
$ContextDest.ExecuteQuery()

Write-Host "Log in to destination SharePoint site " $SiteUrlDest " successfull"

Write-Host "Loading Source role definitions"

#Load the Role Defintions
$Context.Load($Context.Web.RoleDefinitions)
$Context.ExecuteQuery()

Write-Host "Successfully loaded Source role definitions"

Write-Host "Loading Destination role definitions"

$ContextDest.Load($ContextDest.Web.RoleDefinitions)
$ContextDest.ExecuteQuery()

Write-Host "Successfully loaded Destination role definitions"

foreach($srcRole in $Context.Web.RoleDefinitions)
{
    $roleExists = 0

    foreach($destRole in $ContextDest.Web.RoleDefinitions)
	{
	    if($srcRole.Name.Equals($destRole.Name))
		{
		    $roleExists++

            write-host "Role "$srcRole.Name" exists"
            write-host "Updating Role "$srcRole.Name

            $destRole.Description = $srcRole.Description
            $destRole.BasePermissions = $srcRole.BasePermissions
            $destRole.Update()
            $ContextDest.ExecuteQuery()
	    }			
	}

	if($roleExists -eq 0)
	{
        write-host "Role "$srcRole.Name" does not exist"
        write-host "Creating Role "$srcRole.Name

        $roleDefinitionCreationInfo = New-Object "Microsoft.SharePoint.Client.RoleDefinitionCreationInformation"
        $roleDefinitionCreationInfo.Name = $srcRole.Name
        $roleDefinitionCreationInfo.Description = $srcRole.Description
        $roleDefinitionCreationInfo.BasePermissions = $srcRole.BasePermissions
        $ContextDest.Web.RoleDefinitions.Add($roleDefinitionCreationInfo)
        $ContextDest.ExecuteQuery()
    }else {
        
    }
}
#Reload the roles
write-host "Reloading Destination role definitions"
$ContextDest.Load($ContextDest.Web.RoleDefinitions)
$ContextDest.ExecuteQuery()
write-host "Successfully loaded Destination role definitions"

#Manage SharePoint Groups
write-host "Loading Source groups"
$groupsSRC = $Context.Web.SiteGroups
$Context.Load($groupsSRC)
$Context.ExecuteQuery()
write-host "Loading Source groups - Successful"

write-host "Loading Destination groups"
$groupsDEST = $ContextDest.Web.SiteGroups
$ContextDest.Load($groupsDEST)
$ContextDest.ExecuteQuery()
write-host "Loading Destination groups - Successful"

foreach($srcGroup in $groupsSRC)
{ 
    if(!$srcGroup.Title.EndsWith("Members") -and !$srcGroup.Title.EndsWith("Owners") -and !$srcGroup.Title.EndsWith("Visitors"))
    {
        $groupExists = 0

        foreach($destGroup in $groupsDEST)
	    {
	        if($srcGroup.Title.Equals($destGroup.Title))
		    {
				write-host "Group with title"$srcGroup.Title" already exists"
		        $groupExists++
		    }			
	    }

	    if($groupExists -eq 0)
	    {
            Write-Host "Group not found: Title "$srcGroup.Title"Description "$srcGroup.Description"ID "$srcGroup.Id #"Owner " $srcGroup.Owner

            Write-Host "Creating Group" $srcGroup.Title

            #Create new Group
            $newGroupInfo = New-Object Microsoft.SharePoint.Client.GroupCreationInformation  
            $newGroupInfo.Title = $srcGroup.Title  
            $newGroupInfo.Description = $srcGroup.Description
            
            $newGroup = $ContextDest.Web.SiteGroups.Add($newGroupInfo)
            $ContextDest.Load($newGroup)  
            $ContextDest.ExecuteQuery()

			Write-Host "Creating Group" $srcGroup.Title "Successful"

            #Find the role
            foreach($destRole in $ContextDest.Web.RoleDefinitions)
	        {
	            if($destRole.Name.Equals("Contribute Restrict Delete"))
		        {
					Write-Host "Adding Contribute Restrict Delete Role for group "$newGroup.Title"on site level"

					$newGroup = $ContextDest.Web.SiteGroups.GetById($newGroup.Id)

					Write-Host "Loading group "$newGroup.Title
					$ContextDest.Load($newGroup)
					$ContextDest.ExecuteQuery()
					Write-Host "Loading group "$newGroup.Title"successful"

                    # Create a role assignment and apply a role.
					Write-Host "Creating role assignment"
                    $roleAssignment = New-Object Microsoft.SharePoint.Client.RoleDefinitionBindingCollection($ContextDest)
                    $roleAssignment.Add($destRole)
					$ContextDest.Load($destRole)
					$ContextDest.Load($roleAssignment)
					$ContextDest.Load($ContextDest.Web.RoleAssignments)
					$ContextDest.Load($ContextDest.Web.RoleAssignments.Add($newGroup, $roleAssignment))
					$ContextDest.Web.Update()
                    $ContextDest.ExecuteQuery()
					Write-Host "Creatomg role assignment successful!"
                }
            }
        }
    }
}