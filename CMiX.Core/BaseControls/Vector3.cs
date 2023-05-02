// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.BaseControls
{
    public partial class Vector3 : ObservableObject, IControl
    {
        public Vector3(Vector3Model vectorXYZModel)
        {
            ID = vectorXYZModel.ID;
            name = vectorXYZModel.Name;

            X = new FloatValue(vectorXYZModel.X);
            Y = new FloatValue(vectorXYZModel.Y);
            Z = new FloatValue(vectorXYZModel.Z);
        }

        public Guid ID { get; set; }

        [ObservableProperty]
        private string name;

        public FloatValue X { get; set; }
        public FloatValue Y { get; set; }
        public FloatValue Z { get; set; }
    }
}
