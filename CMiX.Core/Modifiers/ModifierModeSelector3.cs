// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;

namespace CMiX.Core.Modifiers
{
    // Does not reuse Integer3 for Count - Integer3 is a plain, generic "3 ints" type with no
    // reason to know about modulator-binding machinery, so each axis gets its own independent
    // ModulatableCount instead of one shared Integer3 whose X/Y/Z would need to become bindable.
    public class ModifierModeSelector3 : IControl
    {
        public ModifierModeSelector3(GenericValue<ModifierMode> mode,
                                     ModulatableCount countX,
                                     ModulatableCount countY,
                                     ModulatableCount countZ)
        {
            Mode = mode;
            CountX = countX;
            CountY = countY;
            CountZ = countZ;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<ModifierMode> Mode { get; set; }
        public ModulatableCount CountX { get; set; }
        public ModulatableCount CountY { get; set; }
        public ModulatableCount CountZ { get; set; }

        public IControlModel ToModel() => new ModifierModeSelector3Model
        {
            ID = ID,
            Mode = (GenericValueModel<ModifierMode>)Mode.ToModel(),
            CountX = (ModulatableCountModel)CountX.ToModel(),
            CountY = (ModulatableCountModel)CountY.ToModel(),
            CountZ = (ModulatableCountModel)CountZ.ToModel()
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
