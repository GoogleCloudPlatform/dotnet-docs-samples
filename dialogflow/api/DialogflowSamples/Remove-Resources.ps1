<#
.DESCRIPTION
ONLY USE FOR CLEANING UP AFTER TESTS.
Removes all the dialogflow resources for a project.
#>

$intents = dotnet run -c Release -- intents:list -p $env:GOOGLE_PROJECT_ID | where {$_ -match "Intent name:.*agent/intents/.*"}
$intentIds = $intents | % { $_ -match "intents/(.*)" | Out-Null; $matches[1] }
while ($intentIds) {
    $group = $intentIds | Select-Object -First 100
    $group | Write-Host
    dotnet run -c Release -- intents:delete -p $env:GOOGLE_PROJECT_ID $group
    $intentIds = $intentIds | Select-Object -Skip 100
}

$entityTypes = dotnet run -c Release -- entity-types:list -p $env:GOOGLE_PROJECT_ID | where {$_ -match "name:.*/agent/entityTypes/.*"}
$entityTypeIds = $entityTypes | % { $_ -match "entityTypes/(.*)" | Out-Null; $matches[1] }
while ($entityTypeIds) {
    $group = $entityTypeIds | Select-Object -First 100
    $group | Write-Host
    dotnet run -c Release -- entity-types:delete -p $env:GOOGLE_PROJECT_ID $group
    $entityTypeIds = $entityTypeIds | Select-Object -Skip 100
}