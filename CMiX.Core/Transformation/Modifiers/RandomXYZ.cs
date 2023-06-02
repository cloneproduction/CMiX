// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Transformation.Modifiers
{
    public partial class RandomXYZ : ObservableObject, IBeatModifiable, ITransformModifier, IEase, ISpreadable
    {
        public RandomXYZ(RandomXYZModel randomXYZModel)
        {
            ID = randomXYZModel.ID;
            Counter = new IntegerValue(randomXYZModel.Counter);
            Visible = new BooleanValue(randomXYZModel.Visible);
            Easing = new Easing(randomXYZModel.Easing);
            BeatModifier = new BeatModifier(randomXYZModel.BeatModifier);
            Mode = new GenericValue<ModifierMode>(randomXYZModel.Mode);
            RandomizeLocation = new BooleanValue(randomXYZModel.RandomizeLocation);
            Location = new Vector3(randomXYZModel.Location);
            RandomizeScale = new BooleanValue(randomXYZModel.RandomizeScale);
            Scale = new Vector3(randomXYZModel.Scale);
            RandomizeRotation = new BooleanValue(randomXYZModel.RandomizeScale);
            Rotation = new Vector3(randomXYZModel.Rotation);
            isExpanded = true;
        }

        public Guid ID { get; set; }
        public BooleanValue Visible { get; set; }
        public GenericValue<ModifierMode> Mode { get; set; }
        public BeatModifier BeatModifier { get; set; }
        public Easing Easing { get; set; }
        public IntegerValue Counter { get; set; }
        public BooleanValue RandomizeLocation { get; set; }
        public Vector3 Location { get; set; }
        public BooleanValue RandomizeScale { get; set; }
        public Vector3 Scale { get; set; }
        public BooleanValue RandomizeRotation { get; set; }
        public Vector3 Rotation { get; set; }

        [ObservableProperty]
        private bool isExpanded;

        [ObservableProperty]
        private bool randomizeLocationIsExpanded;

        [ObservableProperty]
        private bool randomizeScaleIsExpanded;

        [ObservableProperty]
        private bool randomizeRotationIsExpanded;

        public void Dispose()
        {
            BeatModifier.Dispose();
        }
    }
}
