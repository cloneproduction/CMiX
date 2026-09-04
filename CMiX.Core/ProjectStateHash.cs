using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CMiX.Core.Compositing;
using CMiX.Core.Persistence;

namespace CMiX.Core
{
    // Fingerprints a Project's current state so two processes can check they hold the same
    // project without transmitting the whole thing.
    public static class ProjectStateHash
    {
        public static string Compute(Project project)
        {
            return Compute((ProjectModel)project.ToModel());
        }

        public static string Compute(ProjectModel model)
        {
            var json = JsonSerializer.Serialize(model, ProjectSerializer.Options);
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(json));
            return Convert.ToHexString(bytes);
        }
    }
}
