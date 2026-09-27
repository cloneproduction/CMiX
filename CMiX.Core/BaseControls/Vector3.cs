// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CMiX.Core.BaseControls
{
    public partial class Vector3 : ObservableObject, IControl
    {
        public Vector3(GenericValue<float> x,
                       GenericValue<float> y,
                       GenericValue<float> z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<float> X { get; set; }
        public GenericValue<float> Y { get; set; }
        public GenericValue<float> Z { get; set; }

        [RelayCommand]
        private void ResetAll()
        {
            X.Reset();
            Y.Reset();
            Z.Reset();
        }

        public IControlModel ToModel() => new Vector3Model
        {
            ID = ID,
            X = (GenericValueModel<float>)X.ToModel(),
            Y = (GenericValueModel<float>)Y.ToModel(),
            Z = (GenericValueModel<float>)Z.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (Vector3Model)model;
            ID = m.ID;
            X.FromModel(m.X);
            Y.FromModel(m.Y);
            Z.FromModel(m.Z);
        }
    }
}
