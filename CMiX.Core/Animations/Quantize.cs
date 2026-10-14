// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Animations
{
    public partial class Quantize : ObservableRecipient, IControl
    {
        public Quantize(GenericValue<bool> isEnabled,
                        GenericValue<int> steps)
        {
            IsEnabled = isEnabled;
            Steps = steps;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<bool> IsEnabled { get; set; }
        public GenericValue<int> Steps { get; set; }

        public IControlModel ToModel() => new QuantizeModel
        {
            ID = ID,
            IsEnabled = (GenericValueModel<bool>)IsEnabled.ToModel(),
            Steps = (GenericValueModel<int>)Steps.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (QuantizeModel)model;
            ID = m.ID;
            IsEnabled.FromModel(m.IsEnabled);
            Steps.FromModel(m.Steps);
        }
    }
}
