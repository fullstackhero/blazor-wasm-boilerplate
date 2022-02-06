$source = "..\..\dotnet-webapi-boilerplate\src\Core\Shared"
$destination = "..\src\Shared"
$excludes = @('bin', 'obj')

Write-Host "Pull changes from the Fullstackhero WebApi Shared Project"
write-Host "---------------------------------------------------------"
Write-Host
Write-Host "WARNING! This will delete everything in the shared project ($destination)"
Write-Host "and then copy over the whole project from the webapi repository ($source)"
Write-Host
Write-Host "Please make sure the fsh webapi repostory is available on your machine next to this one!"
Write-Host
Read-Host -Prompt "Press ENTER to continue"

Remove-Item -Path "$destination\*"  -Recurse
Copy-Item -Path (Get-Item -Path "$source\*" -Exclude $excludes).FullName -Destination $destination -Recurse -Force

Write-Host "Changes have been pulled."
Write-Host