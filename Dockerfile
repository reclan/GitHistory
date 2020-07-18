FROM mcr.microsoft.com/dotnet/core/sdk:3.1-focal AS build
WORKDIR /build

COPY ./GitHistory /build/
RUN dotnet restore GitHistory.csproj
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-focal AS runtime

RUN apt-get update
RUN apt-get install git -y
                  

WORKDIR /app

COPY --from=build /build/out ./
# COPY /build/out ./
RUN ls

ENTRYPOINT ["dotnet", "GitHistory.dll"]
