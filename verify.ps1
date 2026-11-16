# Restores, formats, builds and tests the solution. Run from the repository root:  ./verify.ps1
# (The cloud session that generated this code had no NuGet access, so this is the first real build.)
$ErrorActionPreference = 'Stop'

dotnet restore ShipFlow.slnx
dotnet format ShipFlow.slnx --no-restore            # fixes whitespace/usings; CI runs --verify-no-changes
dotnet build ShipFlow.slnx --no-restore -c Release
dotnet test ShipFlow.slnx --no-build -c Release      # integration tests need Docker running

Write-Host ''
Write-Host 'Optional: create the first EF migration (replaces EnsureCreated in DbInitializerService):'
Write-Host '  dotnet tool install --global dotnet-ef'
Write-Host '  dotnet ef migrations add InitialCreate -p src/Services/Ordering/ShipFlow.Ordering.Persistence -s src/Services/Ordering/ShipFlow.Ordering.Api -o Migrations'
