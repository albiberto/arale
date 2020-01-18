$ENV = $args[0]
$EndPoint = $args[1]
$BaseUrl = $args[2]
$SpreadsheetId = $args[3]
$Pattern = $args[4]
$ServiceAccountEmail = $args[5]
$Certificate = $args[6]
$Password = $args[7]

if (([string]::IsNullOrEmpty($ENV))) {
    $ENV = "Development"
}

Write-Output "ENV: $ENV"
Write-Output "EndPoint: $EndPoint"
Write-Output "BaseUrl: $BaseUrl"
Write-Output "SpreadSheetId: $SpreadSheetId"
Write-Output "Pattern: $Pattern"
Write-Output "ServiceAccountEmail: $ServiceAccountEmail"
Write-Output "Certificate: $CertificateV"
Write-Output "Password: $Password"

$folder=".\bin\dist-docker-image"

Write-Output "Dist folder: $folder"

if (Test-Path -LiteralPath $folder) {
    # Delete dist folder
    Remove-Item $folder -Force -Recurse
    Write-Output "Dist folder deleted."
}

New-Item -Path $folder -ItemType Directory
Write-Output "Dist folder created."

# Generate .exe
dotnet publish --configuration Release --self-contained -r linux-x64 -o $folder

Write-Output "Build completed."

# Remove and Build Docker Image.
docker rmi -f arale
docker build -t arale .

Write-Output "Docker image builded"

# Remove and Install Docker Container.
docker rm -f araleservice
docker run `
    -e "ENV=$ENV" `
    -e "EndPoint=$EndPoint" `
    -e "BaseUrl=$BaseUrl" `
    -e "SpreadsheetId=$SpreadsheetId" `
    -e "Pattern=$Pattern" `
    -e "ServiceAccountEmail=$ServiceAccountEmail" `
    -e "Certificate=$Certificate" `
    -e "Password=$Password" `
    -ti --name araleservice arale

Write-Output "Arale started."
