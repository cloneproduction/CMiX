// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.BaseControls
{
    public partial class Integer2 : ObservableRecipient, IControl
    {
        public Integer2(GenericValue<int> x,
                        GenericValue<int> y)
        {
            X = x;
            Y = y;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<int> X { get; set; }
        public GenericValue<int> Y { get; set; }

        [RelayCommand]
        private void ResetAll()
        {
            X.Reset();
            Y.Reset();
        }

        public IControlModel ToModel() => new Integer2Model
        {
            ID = ID,
            X = (GenericValueModel<int>)X.ToModel(),
            Y = (GenericValueModel<int>)Y.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (Integer2Model)model;
            ID = m.ID;
            X.FromModel(m.X);
            Y.FromModel(m.Y);
        }
    }
}
