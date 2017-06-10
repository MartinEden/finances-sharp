FROM mono:5.0.0

# Install xsp
RUN apt-get update
RUN apt-get install mono-complete -y

# Create target directories
WORKDIR /app
RUN mkdir ./FinancesSharp
RUN mkdir ./FinancesSharp.Tests/

# First, just copy across the minimum needed to do a NuGet package restore.
# This means Docker will cache the packages unless one of these five files
# changes
COPY FinancesSharp.sln .
COPY FinancesSharp/FinancesSharp.csproj ./FinancesSharp
COPY FinancesSharp/packages.config ./FinancesSharp
COPY FinancesSharp.Tests/FinancesSharp.Tests.csproj ./FinancesSharp.Tests
COPY FinancesSharp.Tests/packages.config ./FinancesSharp.Tests
RUN nuget restore FinancesSharp.sln

# Copy across the code and compile it
COPY FinancesSharp/ ./FinancesSharp/
COPY FinancesSharp.Tests/ ./FinancesSharp.Tests/
RUN msbuild FinancesSharp.sln

CMD cd /app/FinancesSharp && xsp