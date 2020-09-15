FROM mcr.microsoft.com/dotnet/core/runtime:3.1
WORKDIR /opt/dnetapp
COPY /publ/* /opt/dnetapp/
ENTRYPOINT ["dotnet", "/opt/dnetapp/AsapNotificationSystem.dll"]
