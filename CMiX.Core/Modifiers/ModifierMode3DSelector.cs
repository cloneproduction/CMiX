// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Modifiers
{
    public class ModifierMode3DSelector : IControl
    {
        public ModifierMode3DSelector(GenericValue<ModifierMode> mode,
                                      Vector3 count)
        {
            Mode = mode;
            Count = count;
        }

        public GenericValue<ModifierMode> Mode { get; set; }
        public Vector3 Count { get; set; }
        public Guid ID { get; set; }
    }
}
