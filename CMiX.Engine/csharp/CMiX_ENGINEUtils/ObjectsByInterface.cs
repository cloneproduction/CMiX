// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace CMiX_ENGINEUtils
{
    [ProcessNode]
    public class ObjectsByInterface
    {
        private const string VlInterfaceSuffix = "_I";

        private IReadOnlyList<object>? _lastObjects;
        private string? _lastPrefix;
        private Dictionary<string, object> _cached = new();

        public void Update(
            out IReadOnlyDictionary<string, object> result,
            IReadOnlyList<object>? objects = null,
            string prefix = "ICMiXService")
        {
            objects ??= Spread<object>.Empty;

            if (!ReferenceEquals(objects, _lastObjects) || prefix != _lastPrefix)
            {
                var dict = new Dictionary<string, object>(objects.Count);
                foreach (var obj in objects)
                {
                    if (obj is null) continue;
                    foreach (var t in obj.GetType().GetInterfaces())
                    {
                        if (!t.Name.StartsWith(prefix, StringComparison.Ordinal))
                            continue;

                        // VL adds an _I suffix to the .NET name of an interface.
                        var key = t.Name.EndsWith(VlInterfaceSuffix, StringComparison.Ordinal)
                            ? t.Name[..^VlInterfaceSuffix.Length]
                            : t.Name;
                        dict[key] = obj;
                    }
                }
                _cached = dict;
                _lastObjects = objects;
                _lastPrefix = prefix;
            }
            result = _cached;
        }
    }
}
