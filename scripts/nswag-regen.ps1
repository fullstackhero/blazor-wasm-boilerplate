$currentDirectory = Get-Location
$rootDirectory = git rev-parse --show-toplevel
$hostDirectory = $rootDirectory + '/src/Host'
$infrastructurePrj = $rootDirectory + '/src/Client.Infrastructure/Client.Infrastructure.csproj'

Write-Host "Make sure you have run the FSH.WebApi project. `n"
Write-Host "Press any key to continue... `n"
$null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown');

Set-Location -Path $hostDirectory
Write-Host "Host Directory is $hostDirectory `n"

<# Run command #>
dotnet build -t:NSwag $infrastructurePrj

Set-Location -Path $currentDirectory
Write-Host -NoNewLine 'NSwag Regenerated. Press any key to continue...';
$null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown');
