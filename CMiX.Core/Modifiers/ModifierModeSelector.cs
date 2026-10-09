// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;

namespace CMiX.Core.Modifiers
{
    public class ModifierModeSelector : IControl
    {
        public ModifierModeSelector(GenericValue<ModifierMode> mode,
                                    ModulatableValue<int> count)
        {
            Mode = mode;
            Count = count;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<ModifierMode> Mode { get; set; }
        public ModulatableValue<int> Count { get; set; }
        public IEnumerable<IModulatorBindable> Bindables => new IModulatorBindable[] { Count };

        public IControlModel ToModel() => new ModifierModeSelectorModel
        {
            ID = ID,
            Mode = (GenericValueModel<ModifierMode>)Mode.ToModel(),
            Count = (ModulatableValueModel<int>)Count.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (ModifierModeSelectorModel)model;
            ID = m.ID;
            Mode.FromModel(m.Mode);
            Count.FromModel(m.Count);
        }
    }
}
