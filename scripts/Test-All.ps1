$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot

dotnet test "$root/QuebecInsider.sln"
Push-Location "$root/frontend"
npm test
Pop-Location
