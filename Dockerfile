FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source

COPY src/SlackAlertOwner.Notifier/*.csproj SlackAlertOwner.Notifier/

RUN dotnet restore SlackAlertOwner.Notifier/SlackAlertOwner.Notifier.csproj

COPY src/SlackAlertOwner.Notifier/ SlackAlertOwner.Notifier/

WORKDIR /source/SlackAlertOwner.Notifier

RUN dotnet build -c release --no-restore
RUN dotnet publish -c release --no-build -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/core/runtime:3.1
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT dotnet SlackAlertOwner.Notifier.dll --environment "$ENV" --endPoint "$EndPoint" --baseUrl "$BaseUrl" --spreadsheetId "$SpreadSheetId" --pattern "$Pattern" --serviceAccountEmail "$ServiceAccountEmail" --certificate "$Certificate" --password "$Password"