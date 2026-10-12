using System.Text.Json;
using System.Text.Json.Serialization;
using CMiX.Core.BaseControls;

namespace CMiX.Core.Serialization
{
    // Writes AssetSelectorModel.FilePath relative to the project folder and reads it back as
    // an absolute path. A path on another drive stays absolute.
    public class AssetPathJsonConverter : JsonConverter<AssetSelectorModel>
    {
        private readonly string _baseDirectory;

        // The options without this converter. They prevent recursion.
        private readonly JsonSerializerOptions _plain;

        public AssetPathJsonConverter(string baseDirectory, JsonSerializerOptions plain)
        {
            _baseDirectory = baseDirectory;
            _plain = plain;
        }

        public override AssetSelectorModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var model = JsonSerializer.Deserialize<AssetSelectorModel>(ref reader, _plain);
            if (model?.FilePath == null) return model;
            return model with { FilePath = model.FilePath with { Value = ToAbsolute(model.FilePath.Value) } };
        }

        public override void Write(Utf8JsonWriter writer, AssetSelectorModel value, JsonSerializerOptions options)
        {
            var model = value.FilePath == null
                ? value
                : value with { FilePath = value.FilePath with { Value = ToRelative(value.FilePath.Value) } };
            JsonSerializer.Serialize(writer, model, _plain);
        }

        private string ToRelative(string path) =>
            string.IsNullOrEmpty(path) || !Path.IsPathRooted(path) ? path : Path.GetRelativePath(_baseDirectory, path);

        private string ToAbsolute(string path) =>
            string.IsNullOrEmpty(path) || Path.IsPathRooted(path) ? path : Path.GetFullPath(Path.Combine(_baseDirectory, path));
    }
}
