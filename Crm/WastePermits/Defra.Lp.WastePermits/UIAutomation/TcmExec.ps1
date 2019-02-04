param
(
    [string]$BuildDirectory = $null,
    [string]$BuildDefinition = $null,
    [string]$BuildNumber = $null,
    [string]$TestEnvironment = $null,
    [string]$Collection = $(throw "The collection URL must be provided."),
    [string]$TeamProject = $(throw "The team project must be provided."),
    [Int]$PlanId = $(throw "The test plan ID must be provided."),
    [Int]$SuiteId = $(throw "The test suite ID must be provided."),
    [Int]$ConfigId = $(throw "The test configuration ID must be provided."),
    [string]$Title = 'Automated UI Tests',
    [string]$SettingsName = $null,
    [Switch]$InconclusiveFailsTests = $false,
    [Switch]$RemoveIncludeParameter = $false,
    [Switch]$SendResultEmail = $false,
    [string]$SmtpServerName = $null,
    [string]$SendEmailFrom = $null,
    [String]$ResultEmailRecipients = $null,
    [Int16]$TestRunWaitDelay = 10,
    [Int16]$TestResultWaitDelay = 10
)

cls

##################################################################################
# Output the logo.
"Microsoft Release Management TcmExec PowerShell Script v12.0"
"Copyright (c) 2013 Microsoft. All rights reserved.`n"

##################################################################################
# Initialize the default script exit code.
$exitCode = 1

##################################################################################
# Output execution parameters.
"Executing with the following parameters:"
"  Build Directory: $BuildDirectory"
"  Build Definition: $BuildDefinition"
"  Build Number: $BuildNumber"
"  Test Environment: $TestEnvironment"
"  Collection: $Collection"
"  Team project: $TeamProject"
"  Plan ID: $PlanId"
"  Suite ID: $SuiteId"
"  Configuration ID: $ConfigId"
"  Title: $Title"
"  Settings Name: $SettingsName"
"  Inconclusive result fails tests: $InconclusiveFailsTests"
"  Remove /include parameter from /create command: $RemoveIncludeParameter"
"  Test run wait delay: $TestRunWaitDelay"

##################################################################################
# Define globally used variables and constants.
# Visual Studio 2013
$vscommtools = [System.Environment]::GetEnvironmentVariable("VS120COMNTOOLS")
if ($vscommtools -eq $null)
{
    # Visual Studio 2012
    $vscommtools = [System.Environment]::GetEnvironmentVariable("VS110COMNTOOLS")
}
if ($vscommtools -eq $null)
{
    # Visual Studio 2010
    $vscommtools = [System.Environment]::GetEnvironmentVariable("VS100COMNTOOLS")
    if ($vscommtools -ne $null)
    {
        if ([string]::IsNullOrEmpty($BuildDirectory))
        {
            $(throw "The build directory must be provided.")
        }
        if (![string]::IsNullOrEmpty($BuildDefinition) -or ![string]::IsNullOrEmpty($BuildNumber))
        {
            $(throw "The build definition and build number parameters may be used only under Visual Studio 2012/2013.")
        }
    }
}
else
{
    if ([string]::IsNullOrEmpty($BuildDefinition) -and [string]::IsNullOrEmpty($BuildNumber) -and [string]::IsNullOrEmpty($BuildDirectory))
    {
        $(throw "You must specify the build directory or the build definition and build number.")
    }
}
$tcmExe = [System.IO.Path]::GetFullPath($vscommtools + "..\IDE\TCM.exe")

##################################################################################
# Ensure TCM.EXE is available in the assumed path.
if ([System.IO.File]::Exists($tcmExe))
{
    ##################################################################################
    # Prepare optional parameters.
    $testEnvironmentParameter = "/testenvironment:""$TestEnvironment"""
    if ([string]::IsNullOrEmpty($TestEnvironment))
    {
        $testEnvironmentParameter = [string]::Empty
    }
    $buildDirectoryParameter = "/builddir:""$BuildDirectory"""
    if ([string]::IsNullOrEmpty($BuildDirectory))
    {
        $buildDirectoryParameter = [string]::Empty
    }
    $buildDefinitionParameter = "/builddefinition:""$BuildDefinition"""
    if ([string]::IsNullOrEmpty($BuildDefinition))
    {
        $buildDefinitionParameter = [string]::Empty
    }    
	if(([string]::IsNullOrEmpty($BuildNumber)) -and (-not [string]::IsNullOrEmpty($BuildDefinition)))
	{
        Write-Host "Extract Build number using BuildDefinition and BuildDirectory"
		$buildNoRegEx="$BuildDefinition.(.*?)\\|$BuildDefinition.(.*?)$"
        
		if ($BuildDirectory -match $buildNoRegEx)
		{
			foreach($MathihngBuildNo in $Matches.Values)
			{
				if(-not $MathihngBuildNo.Contains("\"))
				{
					$BuildNumber= $MathihngBuildNo
				}
			}
    
			Write-Host "Extracted Build Number: $BuildNumber"
		}
	}
	$buildNumberParameter = "/build:""$BuildNumber"""
	if ([string]::IsNullOrWhiteSpace($BuildNumber))
    {
        $buildNumberParameter = [string]::Empty
    }
    $includeParameter = '/include'
    if ($RemoveIncludeParameter)
    {
        $includeParameter = [string]::Empty
    }
    $settingsNameParameter = "/settingsname:""$SettingsName"""
    if ([string]::IsNullOrEmpty($SettingsName))
    {
        $settingsNameParameter = [string]::Empty
    }

    ##################################################################################
    # Create the test run.
    "`nCreating test run ..."
    $testRunId = & "$tcmExe" run /create /title:"$Title" /planid:$PlanId /suiteid:$SuiteId /configid:$ConfigId /collection:"$Collection" /teamproject:"$TeamProject" $testEnvironmentParameter $buildDirectoryParameter $buildDefinitionParameter $buildNumberParameter $settingsNameParameter $includeParameter
    if ($testRunId -match '.+\:\s(?<TestRunId>\d+)\.')
    {
        # The test run ID is identified as a property in the match collection
        # so we can access it directly by using the group name from the regular
        # expression (i.e. TestRunId).
        $testRunId = $matches.TestRunId

        "Waiting for test run $testRunId to complete ..."
        $waitingForTestRunCompletion = $true
        while ($waitingForTestRunCompletion)
        {
            Start-Sleep -s $TestRunWaitDelay
			# We check if the test run is finished by ensuring the date completed is set. It is not possible to do a "is not null" in WIQL so we use a dummy low date to compare it.
            $testRunStatus = & "$tcmExe" run /list /collection:"$collection" /teamproject:"$TeamProject" /querytext:"SELECT * FROM TestRun WHERE TestRunId=$testRunId and CompleteDate>'1900-01-01'"
			# If there is more than 2 lines, it means there was a result to the query, meaning the run is completed.
            if ($testRunStatus.Count -gt 2)
            {
                $waitingForTestRunCompletion = $false
            }
        }

        "Evaluating test run $testRunId results..."
        # We do a small pause since the results might not be published yet.
        Start-Sleep -s $TestResultWaitDelay

        $testRunResultsTrxFileName = "TestRunResults$testRunId.trx"
        & "$tcmExe" run /export /id:$testRunId /collection:"$collection" /teamproject:"$TeamProject" /resultsfile:"$testRunResultsTrxFileName" | Out-Null
        if ([System.IO.File]::Exists($testRunResultsTrxFileName))
        {
            # Load the XML document contents.
            [xml]$testResultsXml = Get-Content "$testRunResultsTrxFileName"
            
            $tabName = "TestResults"
            #Create Table object
            $ResultTable = New-Object system.Data.DataTable “$tabName”

            #Define Columns
            $colTestName = New-Object system.Data.DataColumn TestName,([string])
            $colOutcome = New-Object system.Data.DataColumn Outcome,([string])

            #Add the Columns
            $ResultTable.columns.add($colTestName)
            $ResultTable.columns.add($colOutcome)

			Write-Host "========================================================================"
			Write-Host "Test Results"
			Write-Host "------------------------------------------------------------------------"

            # Extract result details
            foreach($TestResult in $testResultsXml.TestRun.Results.UnitTestResult)
            {
                Write-Host "Test:" $TestResult.testName "Outcome:" $TestResult.outcome

                #Create a row
                $row = $ResultTable.NewRow()

                #Enter data in the row
                $row.TestName = $TestResult.testName
                $row.Outcome =  $TestResult.outcome

                #Add the row to the table
                $ResultTable.Rows.Add($row)
            }
            
            # Extract the results of the test run.
            $total = $testResultsXml.TestRun.ResultSummary.Counters.total
            $passed = $testResultsXml.TestRun.ResultSummary.Counters.passed
            $failed = $testResultsXml.TestRun.ResultSummary.Counters.failed
            $inconclusive = $testResultsXml.TestRun.ResultSummary.Counters.inconclusive

			Write-Host "------------------------------------------------------------------------"
            # Output the results of the test run.
            "`n========== Test: $total tests ran, $passed succeeded, $failed failed, $inconclusive inconclusive =========="
			Write-Host "========================================================================"

            # Send email if the email send flag is set
            if($SendResultEmail)
            {
                # Style for results table
                $style = "<style>BODY{font-family: Arial; font-size: 10pt;}"
                $style = $style + "TABLE{border: 1px solid black; border-collapse: collapse;}"
                $style = $style + "TH{border: 1px solid black; background: #dddddd; padding: 5px; }"
                $style = $style + "TD{border: 1px solid black; padding: 5px; }"
                $style = $style + "</style>"

				# Create email message 
                $messageBody = $ResultTable | Select-Object TestName, Outcome | ConvertTo-HTML -head $style | Out-String
                # Apply Pass, Fail, Inconclusive styles
                $messageBody = $messageBody -replace "<td>Passed</td>", "<td style='color: green'>Passed</td>" -replace "<td>Failed</td>","<td style='color: red'>Failed</td>" -replace "<td>Inconclusive</td>","<td style='color: orange'>Inconclusive</td>"
                # Add summary line to email
                $messageBody = "$messageBody </br><div>========== Test: $total tests ran, <span style='color:green'>$passed</span> succeeded, <span style='color:red'>$failed</span> failed, <span style='color:orange'>$inconclusive</span> inconclusive ==========</div>"

                # Send email
				$Recipients = [regex]::split($ResultEmailRecipients, ";")
                Send-MailMessage -From $SendEmailFrom -To $Recipients -SmtpServer $SmtpServerName -Body $messageBody -Subject "Test run: $testRunId results for: $Title, on build: $BuildNumber" -BodyAsHtml

                # If sending results email, avoid failing the RM step for test failures
                $exitCode = 0
            }

            # Determine if there were any failed tests during the test run execution.
            elseif ($failed -eq 0 -and (-not $InconclusiveFailsTests -or $inconclusive -eq 0))
            {
                # Update this script's exit code.
                $exitCode = 0
            }

            # Remove the test run results file.
            [System.IO.File]::Delete($testRunResultsTrxFileName) | Out-Null
        }
        else
        {
            "`nERROR: Unable to export test run results file for analysis."
        }
    }
}
else
{
    "`nERROR: Unable to locate $tcmExe"
}

##################################################################################
# Indicate the resulting exit code to the calling process.
if ($exitCode -gt 0)
{
    "`nERROR: Operation failed with error code $exitCode."
}
"`nDone."
exit $exitCode

# SIG # Begin signature block
# MIIh8AYJKoZIhvcNAQcCoIIh4TCCId0CAQExDzANBglghkgBZQMEAgEFADB5Bgor
# BgEEAYI3AgEEoGswaTA0BgorBgEEAYI3AgEeMCYCAwEAAAQQH8w7YFlLCE63JNLG
# KX7zUQIBAAIBAAIBAAIBAAIBADAxMA0GCWCGSAFlAwQCAQUABCBwlIQqpN+6yISm
# wJgrmYhOgqU/xKIq21ht5BEL/OSD/qCCC4MwggULMIID86ADAgECAhMzAAAAM1b2
# lB2ajL3lAAAAAAAzMA0GCSqGSIb3DQEBCwUAMH4xCzAJBgNVBAYTAlVTMRMwEQYD
# VQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNy
# b3NvZnQgQ29ycG9yYXRpb24xKDAmBgNVBAMTH01pY3Jvc29mdCBDb2RlIFNpZ25p
# bmcgUENBIDIwMTAwHhcNMTMwOTI0MTczNTU1WhcNMTQxMjI0MTczNTU1WjCBgzEL
# MAkGA1UEBhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1v
# bmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjENMAsGA1UECxMETU9Q
# UjEeMBwGA1UEAxMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMIIBIjANBgkqhkiG9w0B
# AQEFAAOCAQ8AMIIBCgKCAQEAs9KaOIfw6Oly8PBcJp2mW2pAcbiYWLBfGneq+Oed
# i8Vc8IrjSTO4bEGak9UTxlyKNykoTjwpF275u22O3FPFEQPJU96Y8PFN7E2x8gh4
# 6ftxxmL9XCqnZGd4YJ+qhW3OPuJq9DLc14DJiKAxmHE69CH3N65QJId20RHix/47
# PaEYkBalXwSZ6JLjG9MJSFwmBVUb3WilzUsPv/XM3lWltHUqcbSZwjsM5NKR2HKK
# +eyHIqxqWb90NUky2K0jSbVnEJgQy9TIljp84OA+7ei+v2Lo4dJ7eAYGodazlE1W
# BQ2vCD7ItSKc/m0QL+tjGxW5kCeRZ/sSHyvcdveB1CphyQIDAQABo4IBejCCAXYw
# HwYDVR0lBBgwFgYIKwYBBQUHAwMGCisGAQQBgjc9BgEwHQYDVR0OBBYEFPBHESyD
# Hm5wg0qUmlqkIi/UPOxLMFEGA1UdEQRKMEikRjBEMQ0wCwYDVQQLEwRNT1BSMTMw
# MQYDVQQFEyozODA3NisxMzVlOTk3ZC0yZmUyLTQ3MWMtYjIxYy0wY2VmNjA1OGU5
# ZjYwHwYDVR0jBBgwFoAU5vxfe7siAFjkck619CF0IzLm76wwVgYDVR0fBE8wTTBL
# oEmgR4ZFaHR0cDovL2NybC5taWNyb3NvZnQuY29tL3BraS9jcmwvcHJvZHVjdHMv
# TWljQ29kU2lnUENBXzIwMTAtMDctMDYuY3JsMFoGCCsGAQUFBwEBBE4wTDBKBggr
# BgEFBQcwAoY+aHR0cDovL3d3dy5taWNyb3NvZnQuY29tL3BraS9jZXJ0cy9NaWND
# b2RTaWdQQ0FfMjAxMC0wNy0wNi5jcnQwDAYDVR0TAQH/BAIwADANBgkqhkiG9w0B
# AQsFAAOCAQEAUCzVYWVAmy0CuJ1srWZf0GzTE7bv6EBw3KVMIUi+aQDV1Cmyip6P
# 0aaVqwn2IU4fZCm9cISyrZvvZtsBgZo427YflDWZwXnJVdOhfnUfXD0Ql0G3/eXq
# nwZrQED6XhbKSWXC6g3R47bWLMO2FxrD+oC81yC5iYGvJFCy+iWW7T7Sp2MMr8nZ
# XUmh7VwqxLmESRL9SG0I1jBJeiw3np61RvhG9K7I3ADQAlAwgs07dOphCztGdya7
# LMU0aPEHo4nShwMWGGISjVayRZ3K3KlQQgWDzrgF4alEgf5eHQObN3ZA01YoN2Ir
# J5IcVCEDiAcMbEMVqFPt6srBJveymDXpPDCCBnAwggRYoAMCAQICCmEMUkwAAAAA
# AAMwDQYJKoZIhvcNAQELBQAwgYgxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNo
# aW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29y
# cG9yYXRpb24xMjAwBgNVBAMTKU1pY3Jvc29mdCBSb290IENlcnRpZmljYXRlIEF1
# dGhvcml0eSAyMDEwMB4XDTEwMDcwNjIwNDAxN1oXDTI1MDcwNjIwNTAxN1owfjEL
# MAkGA1UEBhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1v
# bmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEoMCYGA1UEAxMfTWlj
# cm9zb2Z0IENvZGUgU2lnbmluZyBQQ0EgMjAxMDCCASIwDQYJKoZIhvcNAQEBBQAD
# ggEPADCCAQoCggEBAOkOZFB5Z7XE4/0JAEyelKz3VmjqRNjPxVhPqaV2fG1FutM5
# krSkHvn5ZYLkF9KP/UScCOhlk84sVYS/fQjjLiuoQSsYt6JLbklMaxUH3tHSwoke
# cZTNtX9LtK8I2MyI1msXlDqTziY/7Ob+NJhX1R1dSfayKi7VhbtZP/iQtCuDdMor
# sztG4/BGScEXZlTJHL0dxFViV3L4Z7klIDTeXaallV6rKIDN1bKe5QO1Y9OyFMjB
# yIomCll/B+z/Du2AEjVMEqa+Ulv1ptrgiwtId9aFR9UQucboqu6Lai0FXGDGtCpb
# nCMcX0XjGhQebzfLGTOAaolNo2pmY3iT1TDPlR8CAwEAAaOCAeMwggHfMBAGCSsG
# AQQBgjcVAQQDAgEAMB0GA1UdDgQWBBTm/F97uyIAWORyTrX0IXQjMubvrDAZBgkr
# BgEEAYI3FAIEDB4KAFMAdQBiAEMAQTALBgNVHQ8EBAMCAYYwDwYDVR0TAQH/BAUw
# AwEB/zAfBgNVHSMEGDAWgBTV9lbLj+iiXGJo0T2UkFvXzpoYxDBWBgNVHR8ETzBN
# MEugSaBHhkVodHRwOi8vY3JsLm1pY3Jvc29mdC5jb20vcGtpL2NybC9wcm9kdWN0
# cy9NaWNSb29DZXJBdXRfMjAxMC0wNi0yMy5jcmwwWgYIKwYBBQUHAQEETjBMMEoG
# CCsGAQUFBzAChj5odHRwOi8vd3d3Lm1pY3Jvc29mdC5jb20vcGtpL2NlcnRzL01p
# Y1Jvb0NlckF1dF8yMDEwLTA2LTIzLmNydDCBnQYDVR0gBIGVMIGSMIGPBgkrBgEE
# AYI3LgMwgYEwPQYIKwYBBQUHAgEWMWh0dHA6Ly93d3cubWljcm9zb2Z0LmNvbS9Q
# S0kvZG9jcy9DUFMvZGVmYXVsdC5odG0wQAYIKwYBBQUHAgIwNB4yIB0ATABlAGcA
# YQBsAF8AUABvAGwAaQBjAHkAXwBTAHQAYQB0AGUAbQBlAG4AdAAuIB0wDQYJKoZI
# hvcNAQELBQADggIBABp071dPKXvEFoV4uFDTIvwJnayCl/g0/yosl5US5eS/z7+T
# yOM0qduBuNweAL7SNW+v5X95lXflAtTx69jNTh4bYaLCWiMa8IyoYlFFZwjjPzwe
# k/gwhRfIOUCm1w6zISnlpaFpjCKTzHSY56FHQ/JTrMAPMGl//tIlIG1vYdPfB9XZ
# cgAsaYZ2PVHbpjlIyTdhbQfdUxnLp9Zhwr/ig6sP4GubldZ9KFGwiUpRpJpsyLcf
# ShoOaanX3MF+0Ulwqratu3JHYxf6ptaipobsqBBEm2O2smmJBsdGhnoYP+jFHSHV
# e/kCIy3FQcu/HUzIFu+xnH/8IktJim4V46Z/dlvRU3mRhZ3V0ts9czXzPK5UslJH
# asCqE5XSjhHamWdeMoz7N4XR3HWFnIfGWleFwr/dDY+Mmy3rtO7PJ9O1Xmn6pBYE
# AackZ3PPTU+23gVWl3r36VJN9HcFT4XG2Avxju1CCdENduMjVngiJja+yrGMbqod
# 5IXaRzNij6TJkTNfcR5Ar5hlySLoQiElihwtYNk3iUGJKhYP12E8lGhgUu/WR5mg
# gEDuFYF3PpzgUxgaUB04lZseZjMTJzkXeIc2zk7DX7L1PUdTtuDl2wthPSrXkizO
# N1o+QEIxpB8QCMJWnL8kXVECnWp50hfT2sGUjgd7JXFEqwZq5tTG3yOalnXFMYIV
# wzCCFb8CAQEwgZUwfjELMAkGA1UEBhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24x
# EDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlv
# bjEoMCYGA1UEAxMfTWljcm9zb2Z0IENvZGUgU2lnbmluZyBQQ0EgMjAxMAITMwAA
# ADNW9pQdmoy95QAAAAAAMzANBglghkgBZQMEAgEFAKCBrjAZBgkqhkiG9w0BCQMx
# DAYKKwYBBAGCNwIBBDAcBgorBgEEAYI3AgELMQ4wDAYKKwYBBAGCNwIBFTAvBgkq
# hkiG9w0BCQQxIgQgxO76LjO5bOdCJdOoeZcybJGr2fHI4mStClLRz4krIdEwQgYK
# KwYBBAGCNwIBDDE0MDKgGIAWAFQAYwBtAEUAeABlAGMALgBwAHMAMaEWgBRodHRw
# Oi8vbWljcm9zb2Z0LmNvbTANBgkqhkiG9w0BAQEFAASCAQBhuYV9Wh/7ZoRSZfAi
# Fsk8xhgN8q8yFF94G7JpTKo095qkqQtci3bReTJ1s60mL6nuMdTLBlO0z5xmHFix
# Pbx7ipyjjtKu0BLyKV6bJKVwJrE2uWxNFuG6ouUtg9PYhhlBpTG6V2q5CFpVJS1Q
# rtJsEOgTGDwKZSL4EiEsiedKORq+GmUtmdRp+x8qYdLNj8RQV/38oT5YUU8pGI2X
# 1jIHFTHwiihJxZTzPHmcUiEOKlvsKazf4h7I2PyPT3OtXD29/V6+cumWUr/VzzTG
# 5wpFG9JAYNohjPa96rruYNhe+UGP4jcPn+0uz+5Lsd66uLULuPmYAkAOLTYt7ELi
# r7UqoYITTTCCE0kGCisGAQQBgjcDAwExghM5MIITNQYJKoZIhvcNAQcCoIITJjCC
# EyICAQMxDzANBglghkgBZQMEAgEFADCCAT0GCyqGSIb3DQEJEAEEoIIBLASCASgw
# ggEkAgEBBgorBgEEAYRZCgMBMDEwDQYJYIZIAWUDBAIBBQAEIB5ZuR2iY0IgoOdw
# 0cPE0KS9+ewoq9YvqGdm6v049XGnAgZS3pLjpAUYEzIwMTQwMjIwMTQyOTIzLjg5
# MlowBwIBAYACAfSggbmkgbYwgbMxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNo
# aW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29y
# cG9yYXRpb24xDTALBgNVBAsTBE1PUFIxJzAlBgNVBAsTHm5DaXBoZXIgRFNFIEVT
# TjpDMEY0LTMwODYtREVGODElMCMGA1UEAxMcTWljcm9zb2Z0IFRpbWUtU3RhbXAg
# U2VydmljZaCCDtAwggZxMIIEWaADAgECAgphCYEqAAAAAAACMA0GCSqGSIb3DQEB
# CwUAMIGIMQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UE
# BxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMTIwMAYD
# VQQDEylNaWNyb3NvZnQgUm9vdCBDZXJ0aWZpY2F0ZSBBdXRob3JpdHkgMjAxMDAe
# Fw0xMDA3MDEyMTM2NTVaFw0yNTA3MDEyMTQ2NTVaMHwxCzAJBgNVBAYTAlVTMRMw
# EQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVN
# aWNyb3NvZnQgQ29ycG9yYXRpb24xJjAkBgNVBAMTHU1pY3Jvc29mdCBUaW1lLVN0
# YW1wIFBDQSAyMDEwMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqR0N
# vHcRijog7PwTl/X6f2mUa3RUENWlCgCChfvtfGhLLF/Fw+Vhwna3PmYrW/AVUycE
# MR9BGxqVHc4JE458YTBZsTBED/FgiIRUQwzXTbg4CLNC3ZOs1nMwVyaCo0UN0Or1
# R4HNvyRgMlhgRvJYR4YyhB50YWeRX4FUsc+TTJLBxKZd0WETbijGGvmGgLvfYfxG
# wScdJGcSchohiq9LZIlQYrFd/XcfPfBXday9ikJNQFHRD5wGPmd/9WbAA5ZEfu/Q
# S/1u5ZrKsajyeioKMfDaTgaRtogINeh4HLDpmc085y9Euqf03GS9pAHBIAmTeM38
# vMDJRF1eFpwBBU8iTQIDAQABo4IB5jCCAeIwEAYJKwYBBAGCNxUBBAMCAQAwHQYD
# VR0OBBYEFNVjOlyKMZDzQ3t8RhvFM2hahW1VMBkGCSsGAQQBgjcUAgQMHgoAUwB1
# AGIAQwBBMAsGA1UdDwQEAwIBhjAPBgNVHRMBAf8EBTADAQH/MB8GA1UdIwQYMBaA
# FNX2VsuP6KJcYmjRPZSQW9fOmhjEMFYGA1UdHwRPME0wS6BJoEeGRWh0dHA6Ly9j
# cmwubWljcm9zb2Z0LmNvbS9wa2kvY3JsL3Byb2R1Y3RzL01pY1Jvb0NlckF1dF8y
# MDEwLTA2LTIzLmNybDBaBggrBgEFBQcBAQROMEwwSgYIKwYBBQUHMAKGPmh0dHA6
# Ly93d3cubWljcm9zb2Z0LmNvbS9wa2kvY2VydHMvTWljUm9vQ2VyQXV0XzIwMTAt
# MDYtMjMuY3J0MIGgBgNVHSABAf8EgZUwgZIwgY8GCSsGAQQBgjcuAzCBgTA9Bggr
# BgEFBQcCARYxaHR0cDovL3d3dy5taWNyb3NvZnQuY29tL1BLSS9kb2NzL0NQUy9k
# ZWZhdWx0Lmh0bTBABggrBgEFBQcCAjA0HjIgHQBMAGUAZwBhAGwAXwBQAG8AbABp
# AGMAeQBfAFMAdABhAHQAZQBtAGUAbgB0AC4gHTANBgkqhkiG9w0BAQsFAAOCAgEA
# B+aIUQ3ixuCYP4FxAz2do6Ehb7Prpsz1Mb7PBeKp/vpXbRkws8LFZslq3/Xn8Hi9
# x6ieJeP5vO1rVFcIK1GCRBL7uVOMzPRgEop2zEBAQZvcXBf/XPleFzWYJFZLdO9C
# EMivv3/Gf/I3fVo/HPKZeUqRUgCvOA8X9S95gWXZqbVr5MfO9sp6AG9LMEQkIjzP
# 7QOllo9ZKby2/QThcJ8ySif9Va8v/rbljjO7Yl+a21dA6fHOmWaQjP9qYn/dxUoL
# kSbiOewZSnFjnXshbcOco6I8+n99lmqQeKZt0uGc+R38ONiU9MalCpaGpL2eGq4E
# QoO4tYCbIjggtSXlZOz39L9+Y1klD3ouOVd2onGqBooPiRa6YacRy5rYDkeagMXQ
# zafQ732D8OE7cQnfXXSYIghh2rBQHm+98eEA3+cxB6STOvdlR3jo+KhIq/fecn5h
# a293qYHLpwmsObvsxsvYgrRyzR30uIUBHoD7G4kqVDmyW9rIDVWZeodzOwjmmC3q
# jeAzLhIp9cAvVCch98isTtoouLGp25ayp0Kiyc8ZQU3ghvkqmqMRZjDTu3QyS99j
# e/WZii8bxyGvWbWu3EQ8l1Bx16HSxVXjad5XwdHeMMD9zOZN+w2/XU/pnR4ZOC+8
# z1gFLu8NoFA12u8JJxzVs341Hgi62jbb01+P3nSISRIwggTaMIIDwqADAgECAhMz
# AAAAKJBnuQSwPG5mAAAAAAAoMA0GCSqGSIb3DQEBCwUAMHwxCzAJBgNVBAYTAlVT
# MRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQK
# ExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xJjAkBgNVBAMTHU1pY3Jvc29mdCBUaW1l
# LVN0YW1wIFBDQSAyMDEwMB4XDTEzMDMyNzIwMTMxM1oXDTE0MDYyNzIwMTMxM1ow
# gbMxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdS
# ZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xDTALBgNVBAsT
# BE1PUFIxJzAlBgNVBAsTHm5DaXBoZXIgRFNFIEVTTjpDMEY0LTMwODYtREVGODEl
# MCMGA1UEAxMcTWljcm9zb2Z0IFRpbWUtU3RhbXAgU2VydmljZTCCASIwDQYJKoZI
# hvcNAQEBBQADggEPADCCAQoCggEBAN2lSL9qSJ1KIZySa97gLdzkY/jMuYnExxu9
# 57XT++2uzzH+82awRAPamWcIWr91BiIRidBnUsz6z5I3Rej68b0z08xz47ghpKAV
# fcsvxAMF2j+Wc9NZ5ZhNC1aGL51H0dZfnZHpx4TZlWsyTLRrAFLgQdM8aXSozsx9
# aJ1SVeZwfxQHop5nsIZE8zM/c13GKPgXX1IBLUQv1u14CLjlOy/AucNLw7c7L9Om
# tZyxSErdMglWoRtLWtOqJicMEkNgxWrX2kANYJiJQbuTc92//sSMW876VSfKTU2a
# b30RbLFHI7BMfGx/BqF0gh9SeBjpBhoWUJrDOt2BgfebJ3Ihf3UCAwEAAaOCARsw
# ggEXMB0GA1UdDgQWBBTcmAidedEEsotHw3HQjOez5rBTnTAfBgNVHSMEGDAWgBTV
# YzpcijGQ80N7fEYbxTNoWoVtVTBWBgNVHR8ETzBNMEugSaBHhkVodHRwOi8vY3Js
# Lm1pY3Jvc29mdC5jb20vcGtpL2NybC9wcm9kdWN0cy9NaWNUaW1TdGFQQ0FfMjAx
# MC0wNy0wMS5jcmwwWgYIKwYBBQUHAQEETjBMMEoGCCsGAQUFBzAChj5odHRwOi8v
# d3d3Lm1pY3Jvc29mdC5jb20vcGtpL2NlcnRzL01pY1RpbVN0YVBDQV8yMDEwLTA3
# LTAxLmNydDAMBgNVHRMBAf8EAjAAMBMGA1UdJQQMMAoGCCsGAQUFBwMIMA0GCSqG
# SIb3DQEBCwUAA4IBAQCCIvO3PWR+Ekv8Jvzg5LfQxBROCf6rVprRWpimvoxBHpS0
# Mr6EtLdFduPvYBgkh6jP6bTRVCm8yuTLEnvA8dQOnzEzGxE/ejvd3QKqGMrKPPqW
# 42y7r7vJhD7H2AyFy3ILARuk9TEREAxFpVpImX7avkWG17pN5IH/01gKdmUGWS/2
# wkLPBMmrG/phnfXzmElwsUnQZMQh6O5gF1N+6wLaaJWL9Qo8AdujtZgUUXSeU+na
# cphmoF8pz4+vH4Kc0+vm8UwbVPjoMtzBFcOsKm50BRaD40SYn8vv6DB5f69SpTof
# 32XHjf4n2EcZkgi0izSOaSc3GgL2kbOVIv8ISCr+oYIDeTCCAmECAQEwgeOhgbmk
# gbYwgbMxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQH
# EwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xDTALBgNV
# BAsTBE1PUFIxJzAlBgNVBAsTHm5DaXBoZXIgRFNFIEVTTjpDMEY0LTMwODYtREVG
# ODElMCMGA1UEAxMcTWljcm9zb2Z0IFRpbWUtU3RhbXAgU2VydmljZaIlCgEBMAkG
# BSsOAwIaBQADFQDzXbQex187Zk5lDt6YBFP2FacfRKCBwjCBv6SBvDCBuTELMAkG
# A1UEBhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQx
# HjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjENMAsGA1UECxMETU9QUjEn
# MCUGA1UECxMebkNpcGhlciBOVFMgRVNOOkIwMjctQzZGOC0xRDg4MSswKQYDVQQD
# EyJNaWNyb3NvZnQgVGltZSBTb3VyY2UgTWFzdGVyIENsb2NrMA0GCSqGSIb3DQEB
# BQUAAgUA1q++OjAiGA8yMDE0MDIxOTIzMzUyMloYDzIwMTQwMjIwMjMzNTIyWjB3
# MD0GCisGAQQBhFkKBAExLzAtMAoCBQDWr746AgEAMAoCAQACAgO8AgH/MAcCAQAC
# AhbOMAoCBQDWsQ+6AgEAMDYGCisGAQQBhFkKBAIxKDAmMAwGCisGAQQBhFkKAwGg
# CjAIAgEAAgMW42ChCjAIAgEAAgMHoSAwDQYJKoZIhvcNAQEFBQADggEBADvekdlZ
# c1VDIJgYMcDfW7GWc+/tAXbJRhI1Hs1D20ouLjXWYkTuY1fCUtF+rlKSxNvRkbce
# IR7OshnB1mkKmi5ki2vDmbEjINYooE0pN72MScDlGEbaKf1J3JcBJ1p2SC9m3wz6
# ejTBWa50Imi/FWfYSwPumaFoe6MfDl5ySxbYvhOiTGu4cLhKfCFzh5lJ52WoMDXC
# D97+20gbLdNLzIYQXKqGbYMjj81WvpW5MHd9aS85OaASzDqcLDxxKY5Q0rUsVhsV
# 3JOyAw+tF09JAD35ikxhipox6Bp4jtx/z/fO7dTboiZpP8v02ZU6U3VrD0nILr36
# wsmN55146zDqVBkxggL1MIIC8QIBATCBkzB8MQswCQYDVQQGEwJVUzETMBEGA1UE
# CBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9z
# b2Z0IENvcnBvcmF0aW9uMSYwJAYDVQQDEx1NaWNyb3NvZnQgVGltZS1TdGFtcCBQ
# Q0EgMjAxMAITMwAAACiQZ7kEsDxuZgAAAAAAKDANBglghkgBZQMEAgEFAKCCATIw
# GgYJKoZIhvcNAQkDMQ0GCyqGSIb3DQEJEAEEMC8GCSqGSIb3DQEJBDEiBCCoBeNN
# RyFBF7Ex9ETcsA3RAwZqkLLsAQAwZWd54CWULzCB4gYLKoZIhvcNAQkQAgwxgdIw
# gc8wgcwwgbEEFPNdtB7HXztmTmUO3pgEU/YVpx9EMIGYMIGApH4wfDELMAkGA1UE
# BhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAc
# BgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEmMCQGA1UEAxMdTWljcm9zb2Z0
# IFRpbWUtU3RhbXAgUENBIDIwMTACEzMAAAAokGe5BLA8bmYAAAAAACgwFgQUs2rm
# WDWSGf4kTBkVqlFtvUgCf8AwDQYJKoZIhvcNAQELBQAEggEAAQ+o6RIb/aHNXt3d
# anF9zoSh6pWPG0mnMPhnXlxBWr/KMZZU/FPeV8AcOZscYpO9c7ICsdq2k2pyFUDE
# /5YaMdAbQbGBxOr7mF96uSVinF8GREOME/BsZ85EtqgFnbQ1sd0YhjNO6FmYbJIe
# 9NyKRB2ocHtTXKBWGhvfjDAavHK8daaxUK4NpASN2Wnin+/x7QzyhFUBcvvPrVag
# EaDYOcR2M5x7qRfMoSJv4XbaK2bCWfXpOuA0tpeSe4Q9MFMjkASt1PbGXmSNDf5S
# TxCQvBfaaCArVXChBiCXv4c+YHl8I3bUGLc+xFGTq6I4ZvGzNVpJ8NhND9Uo+nBS
# znJ/yQ==
# SIG # End signature block
