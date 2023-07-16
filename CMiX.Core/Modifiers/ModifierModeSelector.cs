// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Modifiers
{
    public class ModifierModeSelector
    {
        public ModifierModeSelector()
        {
            Mode = new GenericValue<ModifierMode>();
            Count = new IntegerValue();
        }

        public GenericValue<ModifierMode> Mode { get; set; }
        public IntegerValue Count { get; set; }
    }
}
