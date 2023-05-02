// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Transformation;
using CMiX.Core.Transformation.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation
{
    public partial class Rotation : ObservableObject, ITransformModifier
    {
        public Rotation(RotationModel rotationModel)
        {
            this.ID = rotationModel.ID;
            XYZ = new Vector3(rotationModel.XYZ);
            Visible = new BooleanValue(rotationModel.Visible);
            Mode = new GenericValue<ModifierMode>(rotationModel.Mode);
            isExpanded = true;
        }

        public Guid ID { get; set; }
        public Vector3 XYZ { get; set; }
        public BooleanValue Visible { get; set; }
        public GenericValue<ModifierMode> Mode  { get; set; }

        [ObservableProperty]
        private bool isExpanded;

        public void Dispose()
        {

        }
    }
}
