// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Modifiers
{
    public class ModifierMode3DSelectorModel : IControlModel
    {
        public ModifierMode3DSelectorModel()
        {
            ID = Guid.NewGuid();
            Mode = new GenericValueModel<ModifierMode>(ModifierMode.PerInstance);
            Count = new Vector3Model(1, 1, 1);
        }

        public Guid ID { get; set; }
        public GenericValueModel<ModifierMode> Mode { get; set; }
        public Vector3Model Count { get; set; }
    }
}
