// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation
{
    public partial class TransformSRT : ObservableObject, IControl, IModifier
    {
        public TransformSRT()
        {
            Uniform = new FloatValue(1.0f);
            Translate = new Translate();
            Scale = new Scale();
            Rotation = new Rotation();
            Visible = new BooleanValue(true);
            Mode = new GenericValue<ModifierMode>();
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
