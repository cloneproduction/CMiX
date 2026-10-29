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

- `Clean`: empties `artifacts`.
- `Publish`: publishes the Studio to `artifacts/CMiX.Studio` and copies the settings file over `studio-settings.json`.
- `Zip`: zips the folder to `artifacts/CMiX.Studio_<version>_win-x64.zip`. This is the default target.
- `Pack`: builds the Chocolatey package in `artifacts` from the zip.
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

## Project files

The Studio saves a project as a `.cmix` file. The file stores the path of every asset relative to its own folder. A project and its assets move together to another machine when the folders between them stay the same. This example has a project and an asset:

    VJ\compositions\bar.cmix
    VJ\textures\foo.jpg

The file stores the asset as `..\textures\foo.jpg`.

An asset on another drive than the project file keeps its absolute path. A project file from an older version keeps its absolute paths until the next save.

The emergency save in the temp folder holds paths relative to that folder. Open it from there and the paths work. Copy the file to another folder before you open it and the paths break.

The start check reports "Not in sync" when you open a project on a machine other than the one that pushed the Redis snapshot, because the absolute paths differ. Click Push on that machine.

## Redis addresses

The Settings tab holds the list of Redis addresses of the project. Click an address to edit it. The buttons at the side add, remove and move addresses. The project file stores the list.

The Redis Float modulator selects one address of the list in its combo box. It has one float output, `Value`. Assign a value to it in the same way as to any other modulator output. The row of the assign menu shows the address text, or `Value` when the modulator selects nothing. A change of the selection keeps the assignments. The Reset menu of the combo box clears the selection. When you delete the selected address, the selection clears and the values bound to the modulator go back to normal in the same undo step.

Each Redis Float modulator has one gain with a start value of 1. The vvvv side multiplies the Redis value of the selected address by the gain. The gain stays when you select another address.

The Redis Bool modulator selects an address in the same way. It has one bool output, `Value`, for a toggle such as the Play of a player. It has no gain. The vvvv side decides how the Redis value becomes a bool.

The list has a fixed manager ID, `ManagerIDs.RedisAddressManager`. The vvvv side creates the list at startup with this ID.

## Hidden types

A class with the attribute `[HiddenInMenu]` is not in the add menus. A saved project can still hold it, and it still works. To show the class again, remove the attribute.

These classes are hidden now: `RenderRandomEntityModifier`, `RenderSequenceEntityModifier` and `BillboardModifier`.

## License

CMiX is free software under the [GNU LGPL, version 3 or later](LICENSE). The GPL text is in [COPYING](COPYING).

## Credits

Created by CloneProduction

Sponsored by Refik Anadol Studio
