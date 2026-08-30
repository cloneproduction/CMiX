// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.Modulation
{
    // The connection record for one Modulatable: which Modulator (into that Modifier's own
    // ModulatorManager) drives it. ModulatorID is what survives a save/load round trip;
    // BoundModulator is a live, non-serialized reference kept alongside it purely for runtime
    // convenience (e.g. the UI reading which instance is bound without a lookup). Both are set
    // together by SetModulator. Null means unbound - the channel's own Value is just its plain
    // edited value, same as any ordinary property. Bound, that same Value is reinterpreted (by
    // the engine, not here) as the modulation depth - there is no separate depth field, the same
    // number just means something different depending on binding state.
    public partial class ChannelBinding : ObservableObject, IControl
    {
        public Guid ID { get; set; } = Guid.NewGuid();

        [ObservableProperty]
        private Guid? modulatorID;

        [ObservableProperty]
        private IModulator boundModulator;

        // Bound to by the channel-assign popup - each listed modulator's item, or null for the
        // popup's "None" entry, is passed straight through as CommandParameter.
        [RelayCommand]
        private void SetModulator(IModulator modulator)
        {
            ModulatorID = modulator?.ID;
            BoundModulator = modulator;
        }

        public IControlModel ToModel() => new ChannelBindingModel
        {
            ID = ID,
            ModulatorID = ModulatorID
        };

        public void FromModel(IControlModel model)
        {
            var m = (ChannelBindingModel)model;
            ID = m.ID;
            ModulatorID = m.ModulatorID;
        }
    }
}
