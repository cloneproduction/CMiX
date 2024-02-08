// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation
{
    public partial class Translate : ObservableObject, IModifier
    {
        public Translate(GenericValue<bool> visible, Vector3 xyz)
        {
            XYZ = xyz;
            Visible = visible;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public Vector3 XYZ { get; set; }
        public GenericValue<bool> Visible { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
