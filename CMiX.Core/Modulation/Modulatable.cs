// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modulation.Modulators;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Modulation
{
    // ModulatorID is what survives a save/load round trip; BoundModulator is a live, non-serialized
    // reference kept alongside it purely for runtime convenience (e.g. the UI reading which instance
    // is bound without a lookup). Both are set together by SetModulator. Null means unbound - the
    // channel's own Value is just its plain edited value, same as any ordinary property. Bound, that
    // same Value is reinterpreted (by the engine, not here) as the modulation depth - there is no
    // separate depth field, the same number just means something different depending on binding state.
    public partial class Modulatable : ObservableObject, IControl
    {
        public Modulatable(GenericValue<float> value,
                           GenericValue<Guid?> modulatorID)
        {
            Value = value;
            ModulatorID = modulatorID;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public string Label { get; set; } = string.Empty;
        public GenericValue<float> Value { get; set; }
        public GenericValue<Guid?> ModulatorID { get; set; }

        [ObservableProperty]
        private IModulator boundModulator;

        // Bound to by the channel-assign popup - each listed modulator's item, or null for the
        // popup's "None" entry, is passed straight through as CommandParameter.
        [RelayCommand]
        private void SetModulator(IModulator modulator)
        {
            BoundModulator = modulator;
            ModulatorID.Value = modulator?.ID;
        }

        public IControlModel ToModel() => new ModulatableModel
        {
            ID = ID,
            Label = Label,
            Value = (GenericValueModel<float>)Value.ToModel(),
            ModulatorID = (GenericValueModel<Guid?>)ModulatorID.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (ModulatableModel)model;
            ID = m.ID;
            Label = m.Label;
            Value.FromModel(m.Value);
            ModulatorID.FromModel(m.ModulatorID);
        }
    }
}
