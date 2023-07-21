// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation
{
    public partial class Scale : ObservableObject, IModifier
    {
        public Scale()
        {
            Uniform = new FloatValue(1.0f);
            XYZ = new Vector3(1.0f, 1.0f, 1.0f);
            Visible = new BooleanValue(true);

            isExpanded = true;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public FloatValue Uniform { get; set; }
        public Vector3 XYZ { get; set; }
        public BooleanValue Visible { get; set; }

        [ObservableProperty]
        private bool isExpanded;
    }
}
