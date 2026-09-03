// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;

namespace CMiX.Core.Modifiers
{
    public class ModifierModeSelector3 : IControl
    {
        public ModifierModeSelector3(GenericValue<ModifierMode> mode,
                                     ModulatableInteger countX,
                                     ModulatableInteger countY,
                                     ModulatableInteger countZ)
        {
            Mode = mode;
            CountX = countX;
            CountY = countY;
            CountZ = countZ;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<ModifierMode> Mode { get; set; }
        public ModulatableInteger CountX { get; set; }
        public ModulatableInteger CountY { get; set; }
        public ModulatableInteger CountZ { get; set; }
        public IEnumerable<IModulatorBindable> Bindables => new IModulatorBindable[] { CountX, CountY, CountZ };

        public IControlModel ToModel() => new ModifierModeSelector3Model
        {
            ID = ID,
            Mode = (GenericValueModel<ModifierMode>)Mode.ToModel(),
            CountX = (ModulatableIntegerModel)CountX.ToModel(),
            CountY = (ModulatableIntegerModel)CountY.ToModel(),
            CountZ = (ModulatableIntegerModel)CountZ.ToModel()
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
