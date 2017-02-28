FROM gcr.io/google-appengine/aspnetcore:1.0.3
COPY . /app
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080
ENTRYPOINT ["dotnet", "RedisCache.dll"]