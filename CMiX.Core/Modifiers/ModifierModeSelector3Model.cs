// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Modifiers
{
    public record ModifierModeSelector3Model : IControlModel
    {
        public ModifierModeSelector3Model()
        {

        }

        public ModifierModeSelector3Model(ModifierMode modifierMode, 
                                          int countX, 
                                          int countY, 
                                          int countZ)
        {
            Mode = new GenericValueModel<ModifierMode>(modifierMode);
            Count = new Integer3Model(countX, countY, countZ);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<ModifierMode> Mode { get; set; } = new(ModifierMode.PerInstance);
        public Integer3Model Count { get; set; } = new(1, 1, 1);
    }
}
