FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ElysiaAPI.sln ./
COPY ElysiaAPI/ElysiaAPI.csproj ElysiaAPI/
COPY ElysiaAPI.Tests/ElysiaAPI.Tests.csproj ElysiaAPI.Tests/
RUN dotnet restore

COPY . .
RUN dotnet build -c Release --no-restore
RUN dotnet test ElysiaAPI.Tests/ElysiaAPI.Tests.csproj -c Release --no-build --logger trx
RUN dotnet publish ElysiaAPI/ElysiaAPI.csproj -c Release -o /app/publish --no-build


FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "ElysiaAPI.dll"]
