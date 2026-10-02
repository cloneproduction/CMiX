// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace CMiX_ENGINEUtils
{
    // Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
    // Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

    using System;

    namespace CMiX_ENGINEUtils
    {
        /// <summary>
        /// Returns the name of the first interface of the object that starts with the prefix.
        /// Returns an empty string when none matches.
        /// </summary>
        [ProcessNode]
        public class InterfaceNameByPrefix
        {
            private const string VlInterfaceSuffix = "_I";

            public void Update(out string name, out bool found, object? obj = null, string prefix = "ICMiXService")
            {
                name = string.Empty;
                found = false;
                if (obj is null)
                    return;

                foreach (var t in obj.GetType().GetInterfaces())
                {
                    if (!t.Name.StartsWith(prefix, StringComparison.Ordinal))
                        continue;

                    name = t.Name.EndsWith(VlInterfaceSuffix, StringComparison.Ordinal)
                        ? t.Name[..^VlInterfaceSuffix.Length]
                        : t.Name;
                    found = true;
                    return;
                }
            }
        }
    }
}
