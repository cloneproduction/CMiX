// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation
{
    public partial class TransformSRT : ObservableObject, IControl, IModifier
    {
        public TransformSRT(FloatValue uniform, Translate translate, Scale scale, Rotation rotation, BooleanValue visible, GenericValue<ModifierMode> mode)
        {
            Uniform = uniform; // new FloatValue(1.0f);
            Translate = translate;
            Scale = scale;
            Rotation = rotation;
            Visible = visible;
            Mode = mode;
            isExpanded = true;
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public FloatValue Uniform { get; set; }
        public Translate Translate { get; set; }
        public Scale Scale { get; set; }
        public Rotation Rotation { get; set; }
        public BooleanValue Visible { get; set; }
        public GenericValue<ModifierMode> Mode { get; set; }

        [ObservableProperty]
        private bool isExpanded;
    }
}
