FROM mcr.microsoft.com/dotnet/runtime:9.0-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
ARG TARGETPLATFORM
ARG BUILDPLATFORM
WORKDIR /src
COPY ["dotnet-aks-workload-identity-sample.csproj", "."]
RUN dotnet restore "dotnet-aks-workload-identity-sample.csproj"
COPY . .
COPY ["Program.cs", "."]
RUN dotnet build "dotnet-aks-workload-identity-sample.csproj" -c Release -o /app/build
RUN apk add unzip
RUN chmod +x /src/download-kubelogin.sh
RUN /src/download-kubelogin.sh $BUILDPLATFORM "v0.2.8"


FROM build AS publish
RUN dotnet publish "dotnet-aks-workload-identity-sample.csproj" -c Release -o /app/publish

FROM base AS final

WORKDIR /app
COPY --from=build /kubelogin/kubelogin .
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "dotnet-aks-workload-identity-sample.dll"]