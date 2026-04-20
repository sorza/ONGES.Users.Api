FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

COPY ONGES.Users.Api/ONGES.Users.Api.csproj ONGES.Users.Api/
COPY ONGES.Users.Application/ONGES.Users.Application.csproj ONGES.Users.Application/
COPY ONGES.Users.Domain/ONGES.Users.Domain.csproj ONGES.Users.Domain/
COPY ONGES.Users.Infrastructure/ONGES.Users.Infrastructure.csproj ONGES.Users.Infrastructure/

RUN dotnet restore ONGES.Users.Api/ONGES.Users.Api.csproj

COPY . .

RUN dotnet publish ONGES.Users.Api/ONGES.Users.Api.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app

RUN addgroup -S appgroup && adduser -S appuser -G appgroup
USER appuser

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ONGES.Users.Api.dll"]