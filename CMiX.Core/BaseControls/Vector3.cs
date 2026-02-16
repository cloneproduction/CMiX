// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.BaseControls
{
    public partial class Vector3 : ObservableObject, IControl
    {
        public Vector3(GenericValue<float> x, GenericValue<float> y, GenericValue<float> z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public IControlModel ToModel() => this.ToModel();
        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<float> X { get; set; }
        public GenericValue<float> Y { get; set; }
        public GenericValue<float> Z { get; set; }
    }
}
