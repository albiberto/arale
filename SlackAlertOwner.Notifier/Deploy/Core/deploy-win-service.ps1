# Delete dist folder
$folder = "C:\apps\arale"

Write-Output "Dist folder: $folder"

if (Test-Path -LiteralPath $folder) {
    # Delete dist folder
    Remove-Item $folder -Force -Recurse
    Write-Output "Dist folder deleted."
}

# Generate .exe
dotnet publish --configuration Release --self-contained -r win10-x64 -o ..\dist-win-service

# Deploy WindowsService.
$params = @{
    Name = "AraleService"
    BinaryPathName = "$folder\SlackAlertOwner.Notifier.exe"
    DisplayName = "Arale"
    StartupType = "Manual"
    Description = "Slack Alert Owner Notifier Service"
}
New-Service @params

# Start Service
Start-Service -Name "AraleService"