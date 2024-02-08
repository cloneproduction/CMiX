// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Diagnostics.Metrics;
using CMiX.Core.BaseControls;

namespace CMiX.Core.Modifiers
{
    public class ModifierModeSelector : IControl
    {
        public ModifierModeSelector(GenericValue<ModifierMode> mode, GenericValue<int> count)
        {
            Mode = mode; // new GenericValue<ModifierMode>();
            Count = count; // new GenericValue<int>(1);
        }

        public GenericValue<ModifierMode> Mode { get; set; }
        public GenericValue<int> Count { get; set; }
        public Guid ID { get; set; }
    }
}
