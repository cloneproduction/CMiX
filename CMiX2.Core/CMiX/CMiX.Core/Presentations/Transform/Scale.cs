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
    public partial class Scale : ObservableObject, ITransformModifier
    {
        public Scale(ScaleModel scaleModel, CompositionService compositionService)
        {
            ID = scaleModel.ID;
            Uniform = new FloatValue(scaleModel.Uniform, compositionService);
            XYZ = new Vector3(scaleModel.XYZ, compositionService);
            Visible = new BooleanValue(scaleModel.Visible, compositionService);
            Mode = new GenericValue<ModifierMode>(scaleModel.Mode, compositionService);
            isExpanded = true;
        }

        public Guid ID { get; set; }
        public FloatValue Uniform { get; set; }
        public Vector3 XYZ { get; set; }
        public BooleanValue Visible { get; set; }
        public GenericValue<ModifierMode> Mode { get; set; }

        [ObservableProperty]
        private bool isExpanded;

        public void Dispose()
        {
            
        }
    }
}
