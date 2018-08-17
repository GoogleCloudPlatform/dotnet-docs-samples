FROM gcr.io/dotnet-debugger/aspnetcore:2.0
COPY . app
WORKDIR /app 
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV ASPNETCORE_URLS=http://*:${PORT}
ENV STACKDRIVER_DEBUGGER_MODULE=module
ENV STACKDRIVER_DEBUGGER_VERSION=1.0
EXPOSE 8080
ENTRYPOINT ["/usr/share/dotnet-debugger/start-debugger.sh", "dotnet", "ProductServiceCore.dll"]
