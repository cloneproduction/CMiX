// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;

namespace CMiX.Core.Modifiers
{
    public class ModifierModeSelector3 : IControl
    {
        public ModifierModeSelector3(GenericValue<ModifierMode> mode,
                                     ModulatableValue<int> countX,
                                     ModulatableValue<int> countY,
                                     ModulatableValue<int> countZ)
        {
            Mode = mode;
            CountX = countX;
            CountY = countY;
            CountZ = countZ;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<ModifierMode> Mode { get; set; }
        public ModulatableValue<int> CountX { get; set; }
        public ModulatableValue<int> CountY { get; set; }
        public ModulatableValue<int> CountZ { get; set; }
        public IEnumerable<IModulatorBindable> Bindables => new IModulatorBindable[] { CountX, CountY, CountZ };

        public IControlModel ToModel() => new ModifierModeSelector3Model
        {
            ID = ID,
            Mode = (GenericValueModel<ModifierMode>)Mode.ToModel(),
            CountX = (ModulatableValueModel<int>)CountX.ToModel(),
            CountY = (ModulatableValueModel<int>)CountY.ToModel(),
            CountZ = (ModulatableValueModel<int>)CountZ.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (ModifierModeSelector3Model)model;
            ID = m.ID;
            Mode.FromModel(m.Mode);
            CountX.FromModel(m.CountX);
            CountY.FromModel(m.CountY);
            CountZ.FromModel(m.CountZ);
        }
    }
}
