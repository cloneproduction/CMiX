// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControl;
using CMiX.Core.Presentations.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.ViewModels.BaseControl
{
    public partial class Vector3 : ObservableObject, IControl
    {
        public Vector3(Vector3Model vectorXYZModel, CompositionService compositionService)
        {
            ID = vectorXYZModel.ID;
            name = vectorXYZModel.Name;

            X = new FloatValue(vectorXYZModel.X, compositionService);
            Y = new FloatValue(vectorXYZModel.Y, compositionService);
            Z = new FloatValue(vectorXYZModel.Z, compositionService);
        }

        public Guid ID { get; set; }

        [ObservableProperty]
        private string name;

        public FloatValue X { get; set; }
        public FloatValue Y { get; set; }
        public FloatValue Z { get; set; }
    }
}
