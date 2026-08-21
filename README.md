# CMiX

CMiX is a VJ (visual jockey) tool. It pairs a desktop Studio application, used
to build and control compositions live, with a vvvv gamma engine that renders
the output. The Studio talks to one or more engine instances over the
network, so composing and rendering can run on separate machines.

## Repo layout

- CMiX.Core: shared domain model and services (compositions, layers, prefabs,
  networking, undo) used by both the Studio and the engine. Built as
  lib/net8.0-windows/CMiX.Core.dll.
- CMiX.Studio.Avalonia: the Studio UI, built on Avalonia. This is where
  active UI work happens.
- CMiX.Engine: the vvvv gamma patch that loads CMiX.Core.dll by binary
  reference and renders the composition output. Because it references the
  built DLL directly rather than the project, the path
  lib/net8.0-windows/CMiX.Core.dll is a contract: vvvv must be closed before
  rebuilding CMiX.Core, otherwise the file is locked and the build fails.
- CMiX.Console: a console host for CMiX.Core, used for headless scenarios.
- CMiX.Core.Tests and CMiX.Studio.Avalonia.Tests: automated tests for the
  shared model and the Avalonia UI respectively.

## Build

    dotnet build CMiX.Studio.Avalonia\CMiX.Studio.Avalonia.csproj

Building CMiX.Core (directly or as a dependency) writes
lib/net8.0-windows/CMiX.Core.dll, so close vvvv first if it has the engine
patch open.

## Test

    dotnet test CMiX.Core.Tests\CMiX.Core.Tests.csproj
    dotnet test CMiX.Studio.Avalonia.Tests\CMiX.Studio.Avalonia.Tests.csproj

## Branches

- feature/avalonia: the WPF to Avalonia UI migration. CMiX.Studio.Avalonia
  reached parity with CMiX.Studio here, which stays in the solution as the
  frozen reference.
- feature/post-parity: incremental improvements on top of the migration,
  including cleanup of dead code, shared markup, and stale project entries.
