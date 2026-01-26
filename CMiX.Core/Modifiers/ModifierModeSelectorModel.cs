// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Modifiers
{
    public record ModifierModeSelectorModel : IControlModel
    {
        public ModifierModeSelectorModel()
        {
            
        }

        public ModifierModeSelectorModel(ModifierMode modifierMode, int count)
        {
            Mode = new GenericValueModel<ModifierMode>(modifierMode);
            Count = new GenericValueModel<int>(count);
        }
        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<ModifierMode> Mode { get; set; } = new(ModifierMode.PerInstance);
        public GenericValueModel<int> Count { get; set; } = new(1);
    }
}
