FROM microsoft/dotnet
WORKDIR /opt/dnetapp
COPY /publ/* /opt/dnetapp/
ENTRYPOINT ["dotnet", "/opt/dnetapp/AsapNotificationSystem.dll"]
