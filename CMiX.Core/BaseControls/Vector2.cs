// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.BaseControls
{
    public partial class Vector2 : ObservableObject, IControl
    {
        public Vector2(GenericValue<float> x, GenericValue<float> y)
        {
            X = x;
            Y = y;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<float> X { get; set; }
        public GenericValue<float> Y { get; set; }

        [RelayCommand]
        private void ResetAll()
        {
            X.Reset();
            Y.Reset();
        }

        public IControlModel ToModel() => new Vector2Model
        {
            ID = ID,
            X = (GenericValueModel<float>)X.ToModel(),
            Y = (GenericValueModel<float>)Y.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (Vector2Model)model;
            ID = m.ID;
            X.FromModel(m.X);
            Y.FromModel(m.Y);
        }
    }
}
