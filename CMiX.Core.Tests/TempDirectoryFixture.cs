using System;
using System.IO;

namespace CMiX.Core.Tests
{
    // A fresh, uniquely named temp directory for one test, deleted when the test disposes it.
    // Each test that needs disk storage creates its own instance, so tests stay isolated from
    // each other the same way they already were before this was extracted.
    public sealed class TempDirectoryFixture : IDisposable
    {
        public string Path { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "CMiX.Core.Tests_" + Guid.NewGuid());

        public TempDirectoryFixture()
        {
            Directory.CreateDirectory(Path);
        }

        public void Dispose()
        {
            if (Directory.Exists(Path))
                Directory.Delete(Path, recursive: true);
        }
    }
}
