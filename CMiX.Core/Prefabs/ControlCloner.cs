// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Text.Json;
using System.Text.RegularExpressions;
using CMiX.Core.Compositing;
using CMiX.Core.Persistence;

namespace CMiX.Core.Prefabs
{
    // Makes a deep copy of a model in which every id is new, so the copy cannot collide with the original.
    public static class ControlCloner
    {
        private const string GuidPattern = @"[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}";

        public static T CloneWithNewGuids<T>(T model) where T : IControlModel =>
            (T)CloneWithNewGuids((IControlModel)model);

        // The copy keeps the runtime type of the model, so a layer model gives a layer model.
        // The output mapping of a composition is a pointer into the fixed output slots, not an identity,
        // and those slots are never cloned. The copy keeps the original value, so a valid selection survives.
        public static IControlModel CloneWithNewGuids(IControlModel model)
        {
            var type = model.GetType();
            var json = ReplaceAllGuids(JsonSerializer.Serialize(model, type, ProjectSerializer.Options));
            var clone = (IControlModel)JsonSerializer.Deserialize(json, type, ProjectSerializer.Options)
                ?? throw new InvalidOperationException($"Failed to clone {type.Name}: deserialization returned null.");

            return clone is CompositionModel composition && model is CompositionModel original
                ? composition with { SelectedOutputMappingID = original.SelectedOutputMappingID }
                : clone;
        }

        private static string ReplaceAllGuids(string json)
        {
            var guidMap = new Dictionary<string, string>();
            return Regex.Replace(json, GuidPattern, match =>
            {
                if (!guidMap.TryGetValue(match.Value, out var newGuid))
                {
                    newGuid = Guid.NewGuid().ToString();
                    guidMap[match.Value] = newGuid;
                }
                return newGuid;
            });
        }
    }
}
