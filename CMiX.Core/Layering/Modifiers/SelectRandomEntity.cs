// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Layering.Modifiers
{
    public class SelectRandomEntity : ObservableObject, IControl, IBeatModifiable, IModifier
    {
        public SelectRandomEntity(GenericValue<bool> visible,
                                  BeatModifier beatModifier,
                                  Easing easing)
        {
            Visible = visible;
            BeatModifier = beatModifier;
            Easing = easing;
        }

        public Guid ID { get; set; }
        public GenericValue<bool> Visible { get; set; }
        public Easing Easing { get; set; }
        public BeatModifier BeatModifier { get; set; }
    }
}
