FROM gcr.io/google-appengine/aspnetcore:2.2
COPY . /app
ENV ASPNETCORE_URLS=http://*:${PORT}
WORKDIR /app
ENTRYPOINT ["dotnet", "SocialAuth.dll"]
