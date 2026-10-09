# CMiX

CMiX is a VJ (visual jockey) tool. A desktop Studio builds and controls compositions. One or more vvvv gamma engines render the output. The Studio and the engines share their state through a Redis server.

## Requirements

- Windows and .NET 8
- A Redis or Memurai server
- vvvv gamma for the engine in `CMiX.Engine`. vvvv has its own license: https://vvvv.org/licensing

## Build

    dotnet build CMiX.Studio.Avalonia\CMiX.Studio.Avalonia.csproj

The notes for developers are in [docs/DEVELOPMENT.md](docs/DEVELOPMENT.md).

## License

CMiX is free software under the [GNU LGPL, version 3 or later](LICENSE). The GPL text is in [COPYING](COPYING).

## Credits

Created by CloneProduction

Additional development: bjorn

Sponsored by Refik Anadol Studio
