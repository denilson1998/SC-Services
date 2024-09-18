FROM mcr.microsoft.com/dotnet/aspnet:6.0
ENV ASPNETCORE_URLS=http://*:8080
ARG DIRR="app/publish"
COPY "${DIRR}" App/
WORKDIR /App
ENTRYPOINT ["dotnet", "Delivery.Api.dll"]