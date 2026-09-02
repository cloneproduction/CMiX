// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation;

namespace CMiX.Core.Modifiers
{
    public class ModifierModeSelector : IControl
    {
        public ModifierModeSelector(GenericValue<ModifierMode> mode,
                                    ModulatableCount count)
        {
            Mode = mode;
            Count = count;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<ModifierMode> Mode { get; set; }
        public ModulatableCount Count { get; set; }

        public IControlModel ToModel() => new ModifierModeSelectorModel
        {
            ID = ID,
            Mode = (GenericValueModel<ModifierMode>)Mode.ToModel(),
            Count = (ModulatableCountModel)Count.ToModel()
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
