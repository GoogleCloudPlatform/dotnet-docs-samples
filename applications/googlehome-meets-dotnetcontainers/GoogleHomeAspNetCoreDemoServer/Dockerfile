FROM gcr.io/google-appengine/aspnetcore:2.0
COPY . /app
WORKDIR /app
EXPOSE 8080 8443
ENV ASPNETCORE_URLS=http://*:8080
ENTRYPOINT ["dotnet", "GoogleHomeAspNetCoreDemoServer.dll"]
