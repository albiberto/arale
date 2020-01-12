# Generate .exe
dotnet publish --configuration Release --self-contained -r win10-x64 -o C:\dist\AraleService

# Copy certificate in dist folder
Copy-Item "certificate.p12" -Destination "C:\dist\AraleService"

# Deploy WindowsService.
$params = @{
    Name = "AraleService"
    BinaryPathName = "C:\dist\AraleService\SlackAlertOwner.Notifier.exe"
    DisplayName = "Arale"
    StartupType = "Manual"
    Description = "Slack Alert Owner Notifier Service"
}
New-Service @params

# Start Service
Start-Service -Name "AraleService"