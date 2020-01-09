dotnet publish --configuration Release --self-contained -r win10-x64 -o ..\dist

New-Service -Name "AraleService" -BinaryPathName "C:\Users\aviez\Projects\SlackAlertOwnerService\dist\SlackAlertOwner.Notifier.exe" -Description "Slack Alert Owner Notifier Service" -DisplayName "Arale" -StartupType Automatic