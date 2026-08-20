using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CMiX.Core.Compositing;
using CMiX.Core.Persistence;

namespace CMiX.Core
{
    // Fingerprints a Project's current state so two independently-running processes can check
    // "do we hold the same project" without transmitting the whole thing. Both sides run the same
    // CMiX.Core code, so hashing the normal serialized output is enough - no cross-language
    // formatting concerns, since there is only one implementation doing the writing either way.
    public static class ProjectStateHash
    {
        public static string Compute(Project project)
        {
            var model = (ProjectModel)project.ToModel();
            var json = JsonSerializer.Serialize(model, ProjectSerializer.Options);
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(json));
            return Convert.ToHexString(bytes);
        }
    }
}
