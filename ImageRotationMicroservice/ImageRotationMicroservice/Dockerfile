FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY ["ImageRotationMicroservice.csproj", "./"]
RUN dotnet restore "./ImageRotationMicroservice.csproj"
COPY . .
RUN dotnet publish "./ImageRotationMicroservice.csproj" -c Release -o ./publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS="http://*:8080"
ENTRYPOINT ["dotnet", "ImageRotationMicroservice.dll"]