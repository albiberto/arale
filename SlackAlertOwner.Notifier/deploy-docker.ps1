param($Env)

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
docker build -t arale --build-arg ENV=$Env .
Write-Output "Docker image builded"

# Remove and Install Docker Container.
docker rm -f araleservice
docker run -t --name araleservice arale

Write-Output "Arale started."

