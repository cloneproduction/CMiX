// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.BaseControls
{
    public partial class FloatRange : ObservableRecipient, IControl
    {
        public FloatRange(GenericValue<float> minimum,
                          GenericValue<float> maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<float> Minimum { get; set; }
        public GenericValue<float> Maximum { get; set; }

        [RelayCommand]
        private void ResetAll()
        {
            Minimum.Reset();
            Maximum.Reset();
        }

        public IControlModel ToModel() => new FloatRangeModel
        {
            ID = ID,
            Minimum = (GenericValueModel<float>)Minimum.ToModel(),
            Maximum = (GenericValueModel<float>)Maximum.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (FloatRangeModel)model;
            ID = m.ID;
            Minimum.FromModel(m.Minimum);
            Maximum.FromModel(m.Maximum);
        }
    }
}
