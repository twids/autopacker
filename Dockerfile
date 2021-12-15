FROM mcr.microsoft.com/dotnet/sdk:latest as build-env
WORKDIR /src

COPY *.csproj .
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish ./AutoPacker.csproj -c Release -o /out --no-self-contained

# Label the container
LABEL maintainer="twids <twidsell@gmail.com>"
LABEL repository="https://github.com/twids/autopacker"

# Label as GitHub action
LABEL com.github.actions.name="Boild"
LABEL com.github.actions.description="This is a work in progress .NET Core Console App to ease cross posting from Hugo to alternate formats."

FROM mcr.microsoft.com/dotnet/runtime:latest

VOLUME /source
VOLUME /destination

ENV AUTOPACKER_WatchDirectory="/source"
ENV AUTOPACKER_UnpackDirectory="/destination"

COPY --from=build-env /out .
ENTRYPOINT [ "dotnet", "./AutoPacker.dll" ]