$ErrorActionPreference = 'Stop'
$destinationPath = 'C:\tools\CMiX.Studio'
$desktopPath = [Environment]::GetFolderPath('Desktop')
$shortcutPath = Join-Path $desktopPath 'CMiX Studio.lnk'

if (Test-Path $destinationPath) {
    Remove-Item -Recurse -Force $destinationPath
}

if (Test-Path $shortcutPath) {
    Remove-Item -Force $shortcutPath
}
