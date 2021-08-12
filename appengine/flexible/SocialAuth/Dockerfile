FROM gcr.io/google-appengine/aspnetcore:3.1
COPY . /app
ENV ASPNETCORE_URLS=http://*:${PORT}
WORKDIR /app
ENTRYPOINT ["dotnet", "SocialAuth.dll"]
