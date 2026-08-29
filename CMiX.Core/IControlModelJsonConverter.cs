using System.Text.Json;
using System.Text.Json.Serialization;

namespace CMiX.Core.Serialization
{
    public class IControlModelJsonConverter : JsonConverter<IControlModel>
    {
        private const string TypeKey = "$type";
        private const string ValueKey = "$value";

        // Build a lookup of short name → concrete type from all assemblies
        private static readonly Dictionary<string, Type> TypeMap =
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => { try { return a.GetTypes(); } catch { return []; } })
                .Where(t => !t.IsAbstract && !t.IsInterface && typeof(IControlModel).IsAssignableFrom(t))
                .ToDictionary(t => t.FullName!, t => t);

        // $type is written as the concrete Model type's FullName, so renaming a Model type breaks
        // deserialization of any project saved under the old name. Add an entry here whenever a
        // *Model type is renamed (property shape unchanged), so already-saved projects still load.
        private static readonly Dictionary<string, string> LegacyTypeNames = new()
        {
            ["CMiX.Core.Transformation.Modifiers.GridModel"] = "CMiX.Core.Transformation.Modifiers.GridLegacyModel",
            ["CMiX.Core.Transformation.Modifiers.CircularSpreadModel"] = "CMiX.Core.Transformation.Modifiers.CircularSpreadLegacyModel",
            ["CMiX.Core.Transformation.Modifiers.LinearXYZModel"] = "CMiX.Core.Transformation.Modifiers.LinearXYZLegacyModel",
            ["CMiX.Core.Modulation.GridModel"] = "CMiX.Core.Modulation.GridModifierModel",
            ["CMiX.Core.Modulation.CircularSpreadModel"] = "CMiX.Core.Modulation.CircularSpreadModifierModel",
            ["CMiX.Core.Modulation.LinearXYZModel"] = "CMiX.Core.Modulation.LinearXYZModifierModel",
            ["CMiX.Core.Transformation.Modifiers.TransformSRTModel"] = "CMiX.Core.Transformation.Modifiers.TransformSRTModifierModel",
        };

        public override IControlModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            if (!root.TryGetProperty(TypeKey, out var typeProp))
                throw new JsonException($"Missing {TypeKey} discriminator");

            var typeName = typeProp.GetString()!;
            if (LegacyTypeNames.TryGetValue(typeName, out var currentTypeName))
                typeName = currentTypeName;

            if (!TypeMap.TryGetValue(typeName, out var concreteType))
                throw new JsonException($"Unknown type: {typeName}");

            return (IControlModel)JsonSerializer.Deserialize(root.GetRawText(), concreteType, options)!;
        }

        public override void Write(Utf8JsonWriter writer, IControlModel value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(TypeKey, value.GetType().FullName);

            // Write all properties of the concrete type
            var json = JsonSerializer.Serialize(value, value.GetType(), options);
            using var doc = JsonDocument.Parse(json);
            foreach (var prop in doc.RootElement.EnumerateObject())
                prop.WriteTo(writer);

            writer.WriteEndObject();
        }
    }
}
