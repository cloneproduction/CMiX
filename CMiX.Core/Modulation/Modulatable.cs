// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Modulation
{
    // One slot on a Modifier - e.g. Scale's X. Not called "Axis": not every future Modifier is
    // XYZ shaped (Color, or a single-value modifier), so Label is a plain string set by the
    // owning concrete Modifier's constructor.
    public class Modulatable : IControl
    {
        public Modulatable(GenericValue<float> value, ChannelBinding binding)
        {
            Value = value;
            Binding = binding;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public string Label { get; set; } = string.Empty;
        public GenericValue<float> Value { get; set; }
        public ChannelBinding Binding { get; set; }

        public IControlModel ToModel() => new ModulatableModel
        {
            ID = ID,
            Label = Label,
            Value = (GenericValueModel<float>)Value.ToModel(),
            Binding = (ChannelBindingModel)Binding.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (ModulatableModel)model;
            ID = m.ID;
            Label = m.Label;
            Value.FromModel(m.Value);
            Binding.FromModel(m.Binding);
        }
    }
}
