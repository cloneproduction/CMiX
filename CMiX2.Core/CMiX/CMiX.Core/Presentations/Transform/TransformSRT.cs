// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Modifiers;
using CMiX.Core.Presentations.Modifiers.Transform;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.ViewModels.BaseControl;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.ViewModels
{
    public partial class TransformSRT : ObservableObject, IControl, IModifier
    {
        public TransformSRT(TransformSRTModel transformModel, CompositionService compositionService)
        {
            isExpanded = true;
            ID = transformModel.ID;
            Visible = new BooleanValue(transformModel.Visible, compositionService);
            Uniform = new FloatValue(transformModel.Uniform, compositionService);
            Translate = new Translate(transformModel.Translate, compositionService);
            Scale = new Scale(transformModel.Scale, compositionService);
            Rotation = new Rotation(transformModel.Rotation, compositionService);
            Mode = new GenericValue<ModifierMode>(transformModel.Mode, compositionService);
        }

        public Guid ID { get; set; }
        public FloatValue Uniform { get; set; }
        public Translate Translate { get; set; }
        public Scale Scale { get; set; }
        public Rotation Rotation { get; set; }
        public BooleanValue Visible { get; set; }
        public GenericValue<ModifierMode> Mode { get; set; }

        [ObservableProperty]
        private bool isExpanded;

        public void Dispose()
        {

        }
    }
}
