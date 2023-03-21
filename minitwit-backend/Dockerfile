FROM mcr.microsoft.com/dotnet/sdk:7.0-bullseye-slim-amd64  AS build-env


ARG TARGETARCH
ARG TARGETOS

RUN arch=$TARGETARCH \
    && if [ "$arch" = "amd64" ]; then arch="x64"; fi \
    && echo $TARGETOS-$arch > /tmp/rid
    
WORKDIR /App

# Copy everything
COPY *.csproj ./
# Restore as distinct layers

RUN dotnet restore
COPY . ./
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /App
COPY --from=build-env /App/out .

EXPOSE 5050

# ENTRYPOINT ["dotnet", "MiniTwit.dll"]
