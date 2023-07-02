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
        public RandomScale(RandomScaleModel randomScaleModel)
        {
            ID = randomScaleModel.ID;
            Visible = new BooleanValue(randomScaleModel.Visible);
            Easing = new Easing(randomScaleModel.Easing);
            BeatModifier = new BeatModifier(randomScaleModel.BeatModifier);
            Scale = new Vector3(randomScaleModel.Scale);
            UniformXYZ = new FloatValue(randomScaleModel.UniformXYZ);
            ModifierModeSelector = new ModifierModeSelector(randomScaleModel.ModifierModeSelector);
            isExpanded = true;
        }

        public Guid ID { get; set; }
        public BooleanValue Visible { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public Easing Easing { get; set; }
        public IntegerValue Counter { get; set; }
        public Vector3 Scale { get; set; }
        public FloatValue UniformXYZ { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }

        [ObservableProperty]
        private bool isExpanded;

        public void Dispose()
        {
            BeatModifier.Dispose();
        }
    }
}
