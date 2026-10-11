$ErrorActionPreference = 'Stop'
$toolsDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$destinationPath = 'C:\tools\CMiX.Studio'
$exePath = Join-Path $destinationPath 'CMiX.Studio.Avalonia.exe'

# The build copies the Studio zip into the tools folder.
$zipRelease = Get-ChildItem -Path $toolsDir -Filter *.zip -File | Select-Object -First 1

New-Item -ItemType Directory -Force -Path $destinationPath | Out-Null
Get-ChocolateyUnzip $zipRelease.FullName $destinationPath

$desktopPath = [Environment]::GetFolderPath('Desktop')
Install-ChocolateyShortcut -ShortcutFilePath (Join-Path $desktopPath 'CMiX Studio.lnk') -TargetPath $exePath
