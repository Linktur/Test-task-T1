FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["T1-test/T1-test.csproj", "T1-test/"]
RUN dotnet restore "T1-test/T1-test.csproj"
COPY . .
WORKDIR "/src/T1-test"
RUN dotnet build "T1-test.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "T1-test.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "T1-test.dll"] 