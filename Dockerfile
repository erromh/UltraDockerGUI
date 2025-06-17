FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
RUN apt-get update && apt-get install -y git
RUN git clone -b main https://github.com/erromh/UltraDockerGUI.git
WORKDIR /src/UltraDockerGUI
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

# Устанавливаем docker-клиент
RUN apt-get update && \
    apt-get install -y docker.io && \
    rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "UltraDockerGUI.dll"]
