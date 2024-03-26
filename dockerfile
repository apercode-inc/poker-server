# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /poker-server

# copy csproj and restore as distinct layers
COPY *.sln .
COPY server/*.csproj ./poker-server/server/
RUN dotnet restore

# copy everything else and build app
COPY server/. ./server/
WORKDIR /source/server

RUN dotnet add package NetFrame -v 1.1.4 
RUN dotnet add package Newtonsoft.Json -v 13.0.3
RUN dotnet add package Scellecs.Morpeh -v 2023.1.0

RUN dotnet publish -c release -o /server --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /server
COPY --from=build /server ./
ENTRYPOINT ["dotnet", "server.dll"]

EXPOSE 5000