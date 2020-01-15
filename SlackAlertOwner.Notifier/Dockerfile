FROM mcr.microsoft.com/dotnet/core/runtime:3.1
ARG ENV=development

WORKDIR /app

COPY ./bin/dist-docker-image/ .

#CMD ["dotnet", "SlackAlertOwner.Notifier.dll", "--environment=$ENV"] 
CMD ["dotnet", "SlackAlertOwner.Notifier.dll"] 