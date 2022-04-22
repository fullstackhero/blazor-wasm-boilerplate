# This script is cross-platform, supporting all OSes that PowerShell Core/7 runs on.

$rootDirectory = git rev-parse --show-toplevel
$sourcePath = Join-Path -Path $rootDirectory -ChildPath '..\dotnet-webapi-boilerplate\src\Core\Shared'
$destinationPath = Join-Path -Path $rootDirectory -ChildPath 'src\Shared'

$excludes = @('bin', 'obj')

Write-Host "Pull changes from the Fullstackhero WebApi Shared Project"
write-Host "---------------------------------------------------------"
Write-Host

If ($null -eq $sourcePath) {
    Write-Error "Error! The expected path of WebApi Shared Project does not exist: $sourcePath"
    Exit 1
}

if ($null -eq (Resolve-Path $destinationPath)) {
    # Ensure the destination exists
    try
    {
        New-Item -Path $destinationPath -ItemType Directory -ErrorAction Stop | Out-Null
    }
    catch
    {
        Write-Error "Error! Unable to create output path \"$destinationPath\""
        Exit 1
    }
}

Write-Host "WARNING! This will delete everything in the shared project ($($destinationPath | Resolve-Path))"
Write-Host "and then copy over the whole project from the webapi repository ($($sourcePath | Resolve-Path))"
Write-Host
Read-Host -Prompt "Press ENTER to continue"

Remove-Item -Path "$destinationPath"  -Recurse -Force
Copy-Item -Path (Get-Item -Path "$sourcePath" -Exclude $excludes).FullName -Destination "$destinationPath" -Recurse -Force

Write-Host "Changes have been pulled."
Write-Host