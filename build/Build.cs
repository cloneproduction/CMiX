using System.IO;
using Fallout.Common;
using Fallout.Common.IO;
using Fallout.Common.Tools.Chocolatey;
using Fallout.Common.Tools.DotNet;
using Serilog;
using static Fallout.Common.Tools.DotNet.DotNetTasks;

class Build : FalloutBuild
{
    public static int Main() => Execute<Build>(x => x.Zip);

    [Parameter("Path of the studio-settings.json to ship. Default: the file in CMiX.Studio.Avalonia.")]
    readonly AbsolutePath SettingsFile;

    [Parameter("URL of the Chocolatey feed")]
    readonly string Feed;

    [Parameter("API key of the Chocolatey feed")] [Secret]
    readonly string ApiKey;

    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath ChocoDirectory => RootDirectory / "choco" / "CMiX.Studio";
    AbsolutePath NuspecFile => ChocoDirectory / "cmix-studio.nuspec";
    AbsolutePath StudioProject => RootDirectory / "CMiX.Studio.Avalonia" / "CMiX.Studio.Avalonia.csproj";
    AbsolutePath DefaultSettingsFile => RootDirectory / "CMiX.Studio.Avalonia" / "studio-settings.json";
    AbsolutePath VersionFile => RootDirectory / "version.txt";
    AbsolutePath PublishDirectory => ArtifactsDirectory / "CMiX.Studio";
    AbsolutePath ZipFile => ArtifactsDirectory / $"CMiX.Studio_{Version}_win-x64.zip";
    AbsolutePath PackageFile => ArtifactsDirectory / $"cmix-studio.{Version}.nupkg";

    string Version => File.ReadAllText(VersionFile).Trim();
    AbsolutePath ShippedSettingsFile => SettingsFile ?? DefaultSettingsFile;

    Target Clean => _ => _
        .Executes(() =>
        {
            ArtifactsDirectory.CreateOrCleanDirectory();
        });

    Target Publish => _ => _
        .DependsOn(Clean)
        .Requires(() => File.Exists(VersionFile))
        .Requires(() => File.Exists(ShippedSettingsFile))
        .Executes(() =>
        {
            DotNetPublish(s => s
                .SetProject(StudioProject)
                .SetConfiguration("Release")
                .SetRuntime("win-x64")
                .SetSelfContained(false)
                .SetOutput(PublishDirectory)
                .SetVersion(Version));

            ShippedSettingsFile.Copy(PublishDirectory / "studio-settings.json", ExistsPolicy.FileOverwrite);
            Log.Information("Shipped settings file: {File}", ShippedSettingsFile);
        });

    Target Zip => _ => _
        .DependsOn(Publish)
        .Produces(ZipFile)
        .Executes(() =>
        {
            PublishDirectory.ZipTo(ZipFile);
        });

    Target Pack => _ => _
        .DependsOn(Zip)
        .Produces(PackageFile)
        .Executes(() =>
        {
            ChocolateyTasks.ChocolateyPack(s => s
                .SetPathToNuspec(NuspecFile)
                .SetOutputDirectory(ArtifactsDirectory)
                .SetVersion(Version));
        });

    Target Push => _ => _
        .DependsOn(Pack)
        .Requires(() => Feed)
        .Requires(() => ApiKey)
        .Executes(() =>
        {
            ChocolateyTasks.ChocolateyPush(s => s
                .SetPathToNuGetPackage(PackageFile)
                .SetSource(Feed)
                .SetApiKey(ApiKey)
                .SetForce(true));
        });
}
