FROM gcr.io/google-appengine/aspnetcore:2.1
COPY . /app
ENV ASPNETCORE_URLS=http://*:${PORT}
WORKDIR /app
ENTRYPOINT ["dotnet", "SocialAuth.dll"]
