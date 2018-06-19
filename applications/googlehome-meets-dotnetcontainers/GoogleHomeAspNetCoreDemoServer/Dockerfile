FROM gcr.io/dotnet-debugger/aspnetcore:2.0
COPY . /app
WORKDIR /app
EXPOSE 8080 8443
ENV ASPNETCORE_URLS=http://*:8080
COPY ./source-context.json /usr/share/dotnet-debugger/agent/
# Uncomment the below two lines if running on GKE
#ENV STACKDRIVER_DEBUGGER=module
#ENV STACKDRIVER_DEBUGGER=1.0
ENTRYPOINT ["/usr/share/dotnet-debugger/start-debugger.sh", "dotnet", "/app/GoogleHomeAspNetCoreDemoServer.dll"]