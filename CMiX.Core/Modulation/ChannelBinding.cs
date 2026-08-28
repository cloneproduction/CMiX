// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Modulation
{
    // The connection record for one Channel: which Modulator (by ID, into that Modifier's own
    // ModulatorManager) drives it and how strongly. ModulatorID null means unbound - the channel
    // just shows its own plain edited value, same as any ordinary property.
    public partial class ChannelBinding : ObservableObject, IControl
    {
        public ChannelBinding(GenericValue<float> depth)
        {
            Depth = depth;
        }

        public Guid ID { get; set; } = Guid.NewGuid();

        [ObservableProperty]
        private Guid? modulatorID;

        public GenericValue<float> Depth { get; set; }

        public IControlModel ToModel() => new ChannelBindingModel
        {
            ID = ID,
            ModulatorID = ModulatorID,
            Depth = (GenericValueModel<float>)Depth.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (ChannelBindingModel)model;
            ID = m.ID;
            ModulatorID = m.ModulatorID;
            Depth.FromModel(m.Depth);
        }
    }
}
