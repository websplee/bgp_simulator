# ============================
#   STAGE 1: Build (.NET 8 SDK)
# ============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution + project files
COPY BGPSimulator_HMC.sln .
COPY BGPSimulator_HMC/ BGPSimulator_HMC/

# Restore & build
RUN dotnet restore
RUN dotnet publish BGPSimulator_HMC/BGPSimulator_HMC.csproj -c Release -o /app/publish

# ============================
#   STAGE 2: Runtime (.NET 8)
# ============================
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app

# Install GraphViz for PNG rendering
RUN apt-get update && apt-get install -y graphviz && rm -rf /var/lib/apt/lists/*

# Copy published output
COPY --from=build /app/publish .

# Ensure graph output folder exists
RUN mkdir -p /app/graphs

# Default command: run the simulator
CMD ["dotnet", "BGPSimulator_HMC.dll"]
