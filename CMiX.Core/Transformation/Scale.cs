// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation
{
    public partial class Scale : ObservableObject, IModifier
    {
        public Scale(GenericValue<float> uniform, Vector3 xyz, GenericValue<bool> visible)
        {
            Uniform = uniform; // new GenericValue<float>(1.0f);
            XYZ = xyz; // new Vector3(1.0f, 1.0f, 1.0f);
            Visible = visible; // new GenericValue<bool>(true);

            isExpanded = true;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public GenericValue<float> Uniform { get; set; }
        public Vector3 XYZ { get; set; }
        public GenericValue<bool> Visible { get; set; }

        [ObservableProperty]
        private bool isExpanded;
    }
}
