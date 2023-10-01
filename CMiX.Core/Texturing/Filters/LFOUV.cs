// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CMiX.Core.Texturing.Sampling;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Filters
{
    public partial class LFOUV : ObservableObject, ITextureModifier, IBeatModifiable
    {
        public LFOUV()
        {
            BeatModifier = new BeatModifier();
            Visible = new BooleanValue(true);
            PingPong = new BooleanValue(false);
            XAxis = new BooleanValue();
            YAxis = new BooleanValue();
            ZAxis = new BooleanValue();
            ModifierModeSelector = new ModifierModeSelector();
            Easing = new Easing();
            From = new FloatValue(0.0f);
            To = new FloatValue(1.0f);
            SamplerState = new SamplerState();
        }

        public Guid ID { get; set; } = Guid.NewGuid();

        public BeatModifier BeatModifier { get; set; }
        public BooleanValue Visible { get; set; }
        public BooleanValue PingPong { get; set; }
        public BooleanValue XAxis { get; set; }
        public BooleanValue YAxis { get; set; }
        public BooleanValue ZAxis { get; set; }
        public ModifierModeSelector ModifierModeSelector { get; set; }
        public Easing Easing { get; set; }
        public FloatValue From { get; set; }
        public FloatValue To { get; set; }
        public SamplerState SamplerState { get; set; }

        [ObservableProperty]
        private bool isExpanded = true;
    }
}
