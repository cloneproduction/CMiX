// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;

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
            CountX = new ModulatableValueModel<int> { Value = new GenericValueModel<int>(countX) };
            CountY = new ModulatableValueModel<int> { Value = new GenericValueModel<int>(countY) };
            CountZ = new ModulatableValueModel<int> { Value = new GenericValueModel<int>(countZ) };
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValueModel<ModifierMode> Mode { get; set; } = new(ModifierMode.PerInstance);
        public ModulatableValueModel<int> CountX { get; set; } = new();
        public ModulatableValueModel<int> CountY { get; set; } = new();
        public ModulatableValueModel<int> CountZ { get; set; } = new();
    }
}
