// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.ViewModels.BaseControl;
using CMiX.Core.Presentations.Beat;
using CMiX.Core.Presentations.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.Modifiers.Transform
{
    public class RandomScale : ObservableObject, IBeatModifiable, ITransformModifier
    {
        public RandomScale(RandomScaleModel randomScaleModel, CompositionService compositionService)
        {
            ID = randomScaleModel.ID;
            Name = randomScaleModel.Name;

            Counter = new IntegerValue(randomScaleModel.CounterModel);
            Visible = new BooleanValue(randomScaleModel.Visible);

            Easing = new Easing(randomScaleModel.EasingModel);
            BeatModifier = new BeatModifier(randomScaleModel.BeatModifierModel, compositionService);

            Mode = new GenericValue<ModifierMode>(randomScaleModel.Mode);

            Scale = new Vector3(randomScaleModel.Scale);
            UniformXYZ = new FloatValue(randomScaleModel.UniformXYZ);

            Spread = new BooleanValue(randomScaleModel.Spread);

            IsExpanded = true;
        }


        public BooleanValue Visible { get; set; }


        public Guid ID { get; set; }
        public TransformModifierNames Name { get; set; }
        public GenericValue<ModifierMode> Mode { get; set; }

        public BeatModifier BeatModifier { get; set; }
        public Easing Easing { get; set; }
        public IntegerValue Counter { get; set; }
        public BooleanValue Spread { get; set; }


        public Vector3 Scale { get; set; }
        public FloatValue UniformXYZ { get; set; }


        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }


        public void Dispose()
        {
            BeatModifier.Dispose();
        }
    }
}
