using System.Text.Json;
using CMiX.Core.Compositing;
using CMiX.Core.Serialization;

namespace CMiX.Core.Persistence
{
    public static class ProjectSerializer
    {
        public static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
            Converters ={new IControlModelJsonConverter()}
        };

        public static void Save(ProjectModel model, string path)
        {
            var json = JsonSerializer.Serialize(model, Options);
            var tempPath = path + ".tmp";
            File.WriteAllText(tempPath, json);
            File.Move(tempPath, path, overwrite: true);
        }

        public static ProjectModel Load(string path)
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<ProjectModel>(json, Options)
                ?? throw new InvalidOperationException("Failed to deserialize project.");
        }
    }
}
