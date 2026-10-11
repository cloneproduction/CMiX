using Fallout.Common;
using Fallout.Common.IO;

class Build : FalloutBuild
{
    public static int Main() => Execute<Build>(x => x.Clean);

    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath ChocoToolsDirectory => RootDirectory / "choco" / "CMiX.Studio" / "tools";

    Target Clean => _ => _
        .Executes(() =>
        {
            ArtifactsDirectory.CreateOrCleanDirectory();
            ChocoToolsDirectory.GlobFiles("*.zip").DeleteFiles();
        });
}
