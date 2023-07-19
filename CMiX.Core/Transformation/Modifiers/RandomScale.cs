// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation.Modifiers
{
    public partial class RandomScale : ObservableObject, IBeatModifiable, IModifier, ISpreadable
    {
        public RandomScale()
        {
            Visible = new BooleanValue(true);
            BeatModifier = new BeatModifier();
            Easing = new Easing();
            Scale = new Vector3();
            UniformXYZ = new FloatValue();
            ModifierModeSelector = new ModifierModeSelector();
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public BooleanValue Visible { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public Easing Easing { get; set; }
        public Vector3 Scale { get; set; }
        public FloatValue UniformXYZ { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
