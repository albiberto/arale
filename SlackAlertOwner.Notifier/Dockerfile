FROM mcr.microsoft.com/dotnet/core/runtime:3.1

WORKDIR /app

COPY ./bin/dist-docker-image/ .

ENTRYPOINT dotnet SlackAlertOwner.Notifier.dll --environment "$ENV" --endPoint "$EndPoint" --baseUrl "$BaseUrl" --spreadsheetId "$SpreadSheetId" --pattern "$Pattern" --serviceAccountEmail "$ServiceAccountEmail" --certificate "$Certificate" --password "$Password"