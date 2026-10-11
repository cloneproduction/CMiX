# CMiX

CMiX is a realtime graphic composition tool.

## Requirements

- Windows and .NET 8
- A Redis or Memurai server
- vvvv gamma for the engine in `CMiX.Engine`. vvvv has its own license: https://vvvv.org/licensing

## Build

    dotnet build CMiX.Studio.Avalonia\CMiX.Studio.Avalonia.csproj

The notes for developers are in [docs/DEVELOPMENT.md](docs/DEVELOPMENT.md).

## Distribution

The Fallout pipeline in `build/Build.cs` publishes the Studio for distribution. The script `build.ps1` restores the tool and runs it. The pipeline needs the .NET 10 SDK. The targets `Pack` and `Push` also need Chocolatey.

Targets:

- `Clean`: empties `artifacts` and deletes the zip in `choco/CMiX.Studio/tools`.
- `Publish`: publishes the Studio to `artifacts/CMiX.Studio` and copies the settings file over `studio-settings.json`.
- `Zip`: zips the folder to `artifacts/CMiX.Studio_<version>_win-x64.zip`. This is the default target.
- `Pack`: copies the zip into `choco/CMiX.Studio/tools` and builds the Chocolatey package in `artifacts`.
- `Push`: pushes the Chocolatey package to a feed.

This command builds `artifacts\CMiX.Studio` and the zip:

    .\build.ps1

This command ships your settings file as `studio-settings.json`. Without the parameter, the file in `CMiX.Studio.Avalonia` ships. Keep a file with project values outside the repository.

    .\build.ps1 --settings-file C:\path\to\studio-settings.json

This command adds the Chocolatey package:

    .\build.ps1 Pack

This command pushes the package. Fallout also reads both values from the environment variables `FEED` and `API_KEY`.

    .\build.ps1 Push --feed <url> --api-key <key>

The file `version.txt` holds the version of the zip, the package and the executable. The package installs the Studio into `C:\tools\CMiX.Studio` and adds a desktop shortcut. The Studio needs the .NET 8 runtime (package `dotnet-8.0-runtime`). An upgrade replaces `studio-settings.json` in that folder with the shipped file.

## License

CMiX is free software under the [GNU LGPL, version 3 or later](LICENSE). The GPL text is in [COPYING](COPYING).

## Credits

Created by CloneProduction

Sponsored by Refik Anadol Studio
