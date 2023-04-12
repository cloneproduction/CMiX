// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.ViewModels.BaseControl;
using CMiX.Core.Presentations.Modifiers.Transform;
using CMiX.Core.Presentations.Transform;
using CommunityToolkit.Mvvm.ComponentModel;
using CMiX.Core.Presentations.Modifiers;
using CMiX.Core.Presentations.Service;

namespace CMiX.Core.Presentations.ViewModels
{
    public partial class Rotation : ObservableObject, ITransformModifier
    {
        public Rotation(RotationModel rotationModel, CompositionService compositionService)
        {
            this.ID = rotationModel.ID;
            XYZ = new Vector3(rotationModel.XYZ, compositionService);
            Visible = new BooleanValue(rotationModel.Visible, compositionService);
            Mode = new GenericValue<ModifierMode>(rotationModel.Mode, compositionService);
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
