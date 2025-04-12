FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY ["ResizeImageMicroservice.csproj", "./"]
RUN dotnet restore "./ResizeImageMicroservice.csproj"
COPY . .
RUN dotnet publish "./ResizeImageMicroservice.csproj" -c Release -o ./publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS="http://*:8080"
ENTRYPOINT ["dotnet", "ResizeImageMicroservice.dll"]
