// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.BaseControls
{
    public partial class Integer3 : ObservableObject, IControl
    {
        public Integer3(GenericValue<int> x, 
                        GenericValue<int> y,
                        GenericValue<int> z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<int> X { get; set; }
        public GenericValue<int> Y { get; set; }
        public GenericValue<int> Z { get; set; }

        public IControlModel ToModel() => new Integer3Model
        {
            ID = ID,
            X = (GenericValueModel<int>)X.ToModel(),
            Y = (GenericValueModel<int>)Y.ToModel(),
            Z = (GenericValueModel<int>)Z.ToModel()
        };

        public void FromModel(IControlModel model)
        {
            var m = (Integer3Model)model;
            ID = m.ID;
            X.FromModel(m.X);
            Y.FromModel(m.Y);
            Z.FromModel(m.Z);
        }
    }
}
