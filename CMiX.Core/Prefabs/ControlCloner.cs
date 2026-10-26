// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using CMiX.Core.Persistence;

namespace CMiX.Core.Prefabs
{
    // Makes a deep copy of a model. The controls inside the copy get new ids, so the copy cannot collide with the original.
    // An id that points to something outside the copy stays as it is: the output mapping of a composition and the
    // Redis addresses of a modulator are examples. A reference to a control inside the copy follows the new id of that control.
    public static class ControlCloner
    {
        private const string GuidPattern = @"[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}";

        public static T CloneWithNewGuids<T>(T model) where T : IControlModel =>
            (T)CloneWithNewGuids((IControlModel)model);

        // The copy keeps the runtime type of the model, so a layer model gives a layer model.
        public static IControlModel CloneWithNewGuids(IControlModel model)
        {
            var type = model.GetType();
            var json = ReplaceOwnGuids(JsonSerializer.Serialize(model, type, ProjectSerializer.Options));

            return (IControlModel)JsonSerializer.Deserialize(json, type, ProjectSerializer.Options)
                ?? throw new InvalidOperationException($"Failed to clone {type.Name}: deserialization returned null.");
        }

        // The ids of the controls inside the model are the values of the properties named ID.
        private static HashSet<string> OwnGuids(JsonNode node)
        {
            var own = new HashSet<string>();
            CollectOwnGuids(node, own);
            return own;
        }

        private static void CollectOwnGuids(JsonNode node, HashSet<string> own)
        {
            switch (node)
            {
                case JsonObject obj:
                    foreach (var pair in obj)
                    {
                        if (pair.Key == "ID" && pair.Value is JsonValue value && value.TryGetValue<string>(out var id) && Guid.TryParse(id, out _))
                            own.Add(id.ToLowerInvariant());
                        else if (pair.Value != null)
                            CollectOwnGuids(pair.Value, own);
                    }
                    break;
                case JsonArray array:
                    foreach (var item in array)
                        if (item != null)
                            CollectOwnGuids(item, own);
                    break;
            }
        }

        private static string ReplaceOwnGuids(string json)
        {
            var own = OwnGuids(JsonNode.Parse(json)!);
            var guidMap = new Dictionary<string, string>();
            return Regex.Replace(json, GuidPattern, match =>
            {
                var key = match.Value.ToLowerInvariant();
                if (!own.Contains(key))
                    return match.Value;

                if (!guidMap.TryGetValue(key, out var newGuid))
                {
                    newGuid = Guid.NewGuid().ToString();
                    guidMap[key] = newGuid;
                }
                return newGuid;
            });
        }
    }
}
