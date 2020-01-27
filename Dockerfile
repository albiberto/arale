FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

COPY *.sln ./
COPY ./src/SlackAlertOwner.Notifier/*.csproj ./src/SlackAlertOwner.Notifier/
COPY ./src/SlackAlertOwner.Tests/*.csproj ./src/SlackAlertOwner.Tests/

RUN dotnet restore

COPY ./src/SlackAlertOwner.Notifier .

RUN dotnet publish -c release -o /dist --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/core/runtime:3.1
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT dotnet SlackAlertOwner.Notifier.dll --environment "$ENV" --endPoint "$EndPoint" --baseUrl "$BaseUrl" --spreadsheetId "$SpreadSheetId" --pattern "$Pattern" --serviceAccountEmail "$ServiceAccountEmail" --certificate "$Certificate" --password "$Password"