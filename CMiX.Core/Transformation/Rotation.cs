// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Transformation;
using CMiX.Core.Transformation.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation
{
    public partial class Rotation : ObservableObject, IModifier
    {
        public Rotation(BooleanValue visible, Vector3 xyz)
        {
            Visible = visible;
            XYZ = xyz;
            isExpanded = true;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public Vector3 XYZ { get; set; }
        public BooleanValue Visible { get; set; }

        [ObservableProperty]
        private bool isExpanded;
    }
}
