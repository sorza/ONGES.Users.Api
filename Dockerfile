FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ONGES.Users.Api/ONGES.Users.Api.csproj ONGES.Users.Api/
COPY ONGES.Users.Application/ONGES.Users.Application.csproj ONGES.Users.Application/
COPY ONGES.Users.Infrastructure/ONGES.Users.Infrastructure.csproj ONGES.Users.Infrastructure/
COPY ONGES.Users.Domain/ONGES.Users.Domain.csproj ONGES.Users.Domain/

RUN dotnet restore "ONGES.Users.Api/ONGES.Users.Api.csproj"

COPY . .
WORKDIR /src/ONGES.Users.Api
RUN dotnet publish "ONGES.Users.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "ONGES.Users.Api.dll"]
