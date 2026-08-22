FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
ENV HUSKY=0
ENV CI=true


COPY *.sln .
COPY Directory.Build.props .
COPY Directory.Build.targets .
COPY src/Tickefy.API/*.csproj src/Tickefy.API/
COPY src/Tickefy.Application/*.csproj src/Tickefy.Application/
COPY src/Tickefy.Infrastructure/*.csproj src/Tickefy.Infrastructure/
COPY src/Tickefy.Domain/*.csproj src/Tickefy.Domain/

COPY tests/Tickefy.API.Tests/*.csproj tests/Tickefy.API.Tests/
COPY tests/Tickefy.Application.Tests/*.csproj tests/Tickefy.Application.Tests/
COPY tests/Tickefy.Domain.Tests/*.csproj tests/Tickefy.Domain.Tests/
COPY tests/Tickefy.Architecture.Tests/*.csproj tests/Tickefy.Architecture.Tests/

RUN dotnet restore

COPY . .

RUN dotnet publish src/Tickefy.API/Tickefy.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Tickefy.API.dll"]
