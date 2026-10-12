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

        // Options for a project file: asset paths are relative to the folder of the file.
        private static JsonSerializerOptions FileOptions(string path)
        {
            var options = new JsonSerializerOptions(Options);
            options.Converters.Add(new AssetPathJsonConverter(Path.GetDirectoryName(Path.GetFullPath(path)), Options));
            return options;
        }

        public static void Save(ProjectModel model, string path)
        {
            var json = JsonSerializer.Serialize(model, FileOptions(path));
            var tempPath = path + ".tmp";
            File.WriteAllText(tempPath, json);
            File.Move(tempPath, path, overwrite: true);
        }

        public static ProjectModel Load(string path)
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<ProjectModel>(json, FileOptions(path))
                ?? throw new InvalidOperationException("Failed to deserialize project.");
        }
    }
}
