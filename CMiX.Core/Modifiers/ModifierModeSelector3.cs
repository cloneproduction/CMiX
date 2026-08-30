// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Modifiers
{
    public class ModifierModeSelector3 : IControl
    {
        public ModifierModeSelector3(GenericValue<ModifierMode> mode,
                                     Integer3 count)
        {
            Mode = mode;
            Count = count;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<ModifierMode> Mode { get; set; }
        public Integer3 Count { get; set; }

        public IControlModel ToModel() => new ModifierModeSelector3Model
        {
            ID = ID,
            Mode = (GenericValueModel<ModifierMode>)Mode.ToModel(),
            Count = (Integer3Model)Count.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (ModifierModeSelector3Model)model;
            ID = m.ID;
            Mode.FromModel(m.Mode);
            Count.FromModel(m.Count);
        }
    }
}
