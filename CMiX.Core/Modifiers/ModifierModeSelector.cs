// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Diagnostics.Metrics;
using CMiX.Core.BaseControls;

namespace CMiX.Core.Modifiers
{
    public class ModifierModeSelector
    {
        public ModifierModeSelector(GenericValue<ModifierMode> mode, IntegerValue count)
        {
            Mode = mode; // new GenericValue<ModifierMode>();
            Count = count; // new IntegerValue(1);
        }

        public GenericValue<ModifierMode> Mode { get; set; }
        public IntegerValue Count { get; set; }
    }
}
