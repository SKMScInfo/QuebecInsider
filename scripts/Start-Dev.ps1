param(
    [int]$ApiPort = 5295,
    [int]$FrontendPort = 4200
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot

Start-Process pwsh -ArgumentList "-NoExit", "-Command", "cd '$root'; `$env:ASPNETCORE_URLS='http://localhost:$ApiPort'; dotnet run --project './backend/src/QuebecInsider.Api'"
Start-Process pwsh -ArgumentList "-NoExit", "-Command", "cd '$root/frontend'; npm start -- --port $FrontendPort"
