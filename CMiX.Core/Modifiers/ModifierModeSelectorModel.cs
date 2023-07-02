// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Modifiers
{
    public class ModifierModeSelectorModel : IControlModel
    {
        public ModifierModeSelectorModel()
        {
            ID = Guid.NewGuid();
            Mode = new GenericValueModel<ModifierMode>(ModifierMode.PerInstance);
            Count = new IntegerValueModel(1);
        }

        public Guid ID { get; set; }
        public GenericValueModel<ModifierMode> Mode { get; set; }
        public IntegerValueModel Count { get; set; }
    }
}
