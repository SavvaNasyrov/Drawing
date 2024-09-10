# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview AS base
RUN apt-get update && apt-get install -y \
    libfontconfig1 \
    libfreetype6 \
    libharfbuzz0b \
    libglib2.0-0 \
    && rm -rf /var/lib/apt/lists/*


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

RUN mkdir -p Drawing
COPY ["./Drawing/Drawing.csproj", "./Drawing/"]
RUN dotnet restore "./Drawing/Drawing.csproj"
COPY . .

WORKDIR "/src/Drawing"
RUN echo $(ls -a)
RUN echo $(pwd)
RUN dotnet build "./Drawing.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Drawing.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Drawing.dll"]