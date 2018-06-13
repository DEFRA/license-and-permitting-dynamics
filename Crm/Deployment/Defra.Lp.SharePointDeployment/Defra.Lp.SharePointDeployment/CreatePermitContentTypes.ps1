#
# CreatePermitContentTypes.ps1
#

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
$Context.Load($Context.Web.AvailableContentTypes)
$Context.ExecuteQuery()

Write-Host "Log in to source SharePoint site " $SiteUrl " successfull"

Write-Host "Log in to destination SharePoint site " $SiteUrlDest

$CredentialsDest = New-Object Microsoft.SharePoint.Client.SharePointOnlineCredentials($UserNameDest,(ConvertTo-SecureString $PasswordDest -AsPlainText -Force)) 
$ContextDest = New-Object Microsoft.SharePoint.Client.ClientContext($SiteUrlDest) 
$ContextDest.Credentials = $CredentialsDest 

$ContextDest.Load($ContextDest.Web.AvailableContentTypes)
#$ContextDest.Load($ContextDest.Web.ContentTypes)
$ContextDest.ExecuteQuery()

Write-Host "Log in to destination SharePoint site " $SiteUrlDest " successfull"

#Get the Content Type Schema from XML
#$XMLFile = "./SiteContentTypes.xml" 
#[xml] $CTypeXML = Get-Content($XMLFile)
  
#Create Site Content Types
#$CTypeXML.ContentTypes.ContentType | ForEach-Object {


#vars


##vars

#Manage the columns / fields (Copies accross all fields from the source to destination if not exist)
#Create the fields from Source if they dont exist on the destination

$fieldsDEST = $ContextDest.Web.Fields
$fieldsSRC = $Context.Web.Fields
#$contentTypesDEST = New-Object Microsoft.SharePoint.Client.ContentTypeCollection
#$contentTypesDEST = $ContextDest.Web.ContentTypes
$ContextDest.load($fieldsDEST)
$Context.Load($fieldsSRC)
#$ContextDest.load($contentTypesDEST)
Write-Host "Loading the destination site fields"
$ContextDest.ExecuteQuery()
Write-Host "Loading the source site fields"
$Context.ExecuteQuery()


#$contentTypesDEST.Add($SPContentType)
#$ContextDest.Load($SPContentType)
#$ContextDest.ExecuteQuery()

Write-Host "Processing fields"    

#array with all fields being created
$newFields = @()
       
foreach($srcField in $fieldsSRC)
{
    if($srcField.Group.Equals("LP"))
    {
        Write-Host "Processing source field " $srcField.InternalName 

	    $fieldExists = 0

	    foreach($dstField in $fieldsDEST)
	    {
		    if($dstField.EntityPropertyName.Equals($srcField.EntityPropertyName))
		    {
			    $fieldExists++
		    }
					
	    }

	    if($fieldExists -eq 0)
	    {
		    Write-Host $srcField.InternalName"Does not exist"

		    #$fieldAsXML = "<Field Type='Text'
		    #DisplayName='$srcField.InternalName'
		    #Name='$srcField.InternalName' 
		    #Group='$srcField.Group'/>"
		    #$fieldOption = [Microsoft.SharePoint.Client.AddFieldOptions]::AddFieldInternalNameHint

		    #$field = $fieldsDEST.AddFieldAsXML($srcField.SchemaXml, $true, $fieldOption)
		    #$ContextDest.load($field)

		    $newFields += $srcField
	    }
    }
}

foreach($newField in $newFields)
{
    #Remove the Version with regex Version="123" attribute from the xml as the creation throws an error otherwise
    $fieldAsXML = $newField.SchemaXml
    $fieldAsXML = $fieldAsXML -replace 'Version="[0-9]+"', ''

    Write-Host "Creating field " $newField.InternalName
    $fieldOption = [Microsoft.SharePoint.Client.AddFieldOptions]::AddFieldInternalNameHint
    $field = $ContextDest.Web.Fields.AddFieldAsXML($fieldAsXML, $true, $fieldOption)
    $ContextDest.load($field)
    $ContextDest.ExecuteQuery()

    #$ContextDest.Load($fieldsDEST);
    #$ContextDest.ExecuteQuery()
}

#Manage the content types
write-host "Processing content types"
foreach($ccSRC in $Context.Web.AvailableContentTypes)
{
	#Process only the LP group content types
	if($ccSRC.Group.Equals("LP"))
	{
		#$lci =New-Object Microsoft.SharePoint.Client.ContentTypeCreationInformation
		#$lci.Description="Description"
		#$lci.Name="Powershell Content Type2"
		#$lci.ParentContentType=$ctx.Web.ContentTypes.GetById("0x01")
		#$lci.Group="List Content Types"

		#Always contains a content type from the destination - either a new content type created below or existing one
		$SPContentType = New-Object Microsoft.SharePoint.Client.ContentTypeCreationInformation

		$found = 0

		foreach($cc in $ContextDest.Web.AvailableContentTypes)
		{
			if($cc.Name.Equals($ccSRC.Name))
			{
			  $found++
			  $SPContentType = $cc
			  write-host "Content type " $cc.Name " with ID "  $cc.Id " found"
			}
		}

		#If not found
		if($found -eq 0)
		{
		  write-host "Content type " $ccSRC.Name " with ID "  $ccSRC.Id " not found"

		  #Get the parent
		  $Context.Load($ccSRC.Parent)
		  $Context.ExecuteQuery()

		  Write-Host "Loading Parent with ID " $ccSRC.Parent.Id


		  #Create the content type
		  #$ContextDest.Web.ContentTypes.AddExistingContentType($ccSRC)
		  #$ContextDest.ExecuteQuery()

		  #Create it
		  #Create New Content Type object inheriting from parent
		  #$SPContentType = New-Object Microsoft.SharePoint.Client.ContentTypeCreationInformation # New-Object Microsoft.SharePoint.SPContentType ($_.ID,$TargetWeb.ContentTypes,$_.Name)
		   $SPContentType = New-Object Microsoft.SharePoint.Client.ContentTypeCreationInformation


		   Write-Host "Type ID " $SPContentType.TypeId

		   #Set Content Type name and id
		   $SPContentType.Name = $ccSRC.Name
		   #$SPContentType.Id = $ccSRC.Id.StringValue

		   #Set Content Type description and group
		   $SPContentType.Description = $ccSRC.Description
		   $SPContentType.Group = $ccSRC.Group


		   $ContextDest.Load($ContextDest.Web.ContentTypes)
		   $ContextDest.ExecuteQuery()
		  
			foreach($ccPar in $ContextDest.Web.ContentTypes)
			{
				if($ccPar.Name.Equals($ccSRC.Parent.Name))
				{
					$SPContentType.ParentContentType = $ccPar
				}
			}

			#Write-Host "test " $SPContentType.ParentContentType
			
			write-host "Content type " $SPContentType.Name " with ID " $SPContentType.Id " is being created"

			#The actual creation
			$ctCreated = $ContextDest.Web.ContentTypes.Add($SPContentType)
			$ContextDest.Load($ctCreated)
			$ContextDest.ExecuteQuery()
		}
		
		#Manage field mappings
		write-host "Processing field mappings of Content type " $SPContentType.Name " with ID "  $SPContentType.Id
		
		#$currentDestContentType = $ContextDest.Web.ContentTypes.GetById($SPContentType.TypeId)

        $ContextDest.Load($ContextDest.Web.ContentTypes)
		$ContextDest.ExecuteQuery()

		$currentDestContentType = New-Object Microsoft.SharePoint.Client.ContentTypeCreationInformation
		foreach($updatedCT in $ContextDest.Web.ContentTypes)
		{
		    if($updatedCT.Name -eq $SPContentType.Name)
		    {
		        $currentDestContentType = $updatedCT
		    }
		}
		
		#Load the CT (new or existing) field links
		#$ContextDest.Load($currentDestContentType)

		$ContextDest.Load($currentDestContentType.FieldLinks)
		$ContextDest.ExecuteQuery()
		
		#Load the source field mappings
		$Context.Load($ccSRC.FieldLinks)
		$Context.ExecuteQuery()

		#Add the field mappings
		foreach($fieldMap in $ccSRC.FieldLinks)
		{
			#Write-Host $fieldMap.Name $fieldMap.Required
			
			$found = 0
			
			foreach($fieldMapDest in $currentDestContentType.FieldLinks)
			{
				if($fieldMapDest.Name -ne $null -and $fieldMapDest.Name.Equals($fieldMap.Name))
				{
					$found++
				}
			}
			
			if($found -eq 0)
			{
				Write-Host "Field Mapping " $fieldMap.Name " for content type " $currentDestContentType.Name " not found"
				Write-Host "Creating Field Mapping " $fieldMap.Name " Required: " $fieldMap.Required
				
				$field = $fieldsDEST.GetByInternalNameOrTitle($fieldMap.Name)
				#create FieldLinkCreationInformation object (flci)
				$flci = new-object Microsoft.SharePoint.Client.FieldLinkCreationInformation
				$flci.Field = $field
				$addContentType = $currentDestContentType.FieldLinks.Add($flci)
				$currentDestContentType.Update($true)
				$ContextDest.ExecuteQuery()
			}
		} 
		#$currentDestContentType.Update($true)
		#$ContextDest.ExecuteQuery()


		#$currentDestContentType.FieldLinks = $ccSRC.FieldLinks
		#$currentDestContentType.Fields = $ccSRC.Fields
		#$currentDestContentType.Update($true)
		#$ContextDest.ExecuteQuery()
		
		# Write-Host $_.Parent
	}
}

#$TargetWeb.Dispose()