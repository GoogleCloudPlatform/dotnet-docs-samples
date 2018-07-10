while ($True) {
    $intents = dotnet run -c Release -- intents:list -p $env:GOOGLE_PROJECT_ID | where {$_ -match "Intent name:.*agent/intents/.*"}
    $intentIds = $intents | % { $_ -match "intents/(.*)" | Out-Null; $matches[1] } | select -first 100
    if (-not $intentIds) { break; }
    $intentIds | Out-Host
    dotnet run -c Release -- intents:delete -p $env:GOOGLE_PROJECT_ID $intentIds
}