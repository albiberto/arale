FROM mcr.microsoft.com/dotnet/core/sdk:3.1

WORKDIR /app

COPY ./src/SlackAlertOwner.Notifier .

RUN dotnet publish --configuration Release --self-contained -r linux-x64 -o .dist

ENTRYPOINT dotnet SlackAlertOwner.Notifier.dll --environment "$ENV" --endPoint "$EndPoint" --baseUrl "$BaseUrl" --spreadsheetId "$SpreadSheetId" --pattern "$Pattern" --serviceAccountEmail "$ServiceAccountEmail" --certificate "$Certificate" --password "$Password"