// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Beat;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.ViewModels.BaseControl;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.Modifiers.Transform
{
    public partial class RandomXYZ : ObservableObject, IBeatModifiable, ITransformModifier
    {
        public RandomXYZ(RandomXYZModel randomXYZModel, CompositionService compositionService)
        {
            ID = randomXYZModel.ID;
            Name = randomXYZModel.Name;
            Counter = new IntegerValue(randomXYZModel.Counter, compositionService);
            Visible = new BooleanValue(randomXYZModel.Visible, compositionService);
            Easing = new Easing(randomXYZModel.Easing, compositionService);
            BeatModifier = new BeatModifier(randomXYZModel.BeatModifier, compositionService);
            Mode = new GenericValue<ModifierMode>(randomXYZModel.Mode, compositionService);
            RandomizeLocation = new BooleanValue(randomXYZModel.RandomizeLocation, compositionService);
            RandomizeLocation.Value = true;
            Location = new Vector3(randomXYZModel.Location, compositionService);
            RandomizeScale = new BooleanValue(randomXYZModel.RandomizeScale, compositionService);
            RandomizeScale.Value = true;
            Scale = new Vector3(randomXYZModel.Scale, compositionService);
            RandomizeRotation = new BooleanValue(randomXYZModel.RandomizeScale, compositionService);
            RandomizeRotation.Value = true;
            Rotation = new Vector3(randomXYZModel.Rotation, compositionService);
            isExpanded = true;
        }

       
        public Guid ID { get; set; }
        public BooleanValue Visible { get; set; }
        public TransformModifierNames Name { get; set; }
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
