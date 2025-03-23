FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app #Устанавливает рабочую директорию внутри контейнера на /app

COPY . . #Копирует все файлы из текущей директории на хосте

RUN dotnet restore  # Восстанавливает зависимости проекта, используя файл *.csproj

RUN dotnet publish -c Release -o out #Собирает проект в режиме Release и публикует его в папку out внутри контейнера

# Финальный образ для запуска приложения
FROM mcr.microsoft.com/dotnet/runtime:8.0

WORKDIR /app

COPY --from=build /app/out . #Копирует собранные файлы из папки out этапа build

CMD ["dotnet", "testRedis.dll"] #Указывает команду, которая будет выполнена при запуске контейнера