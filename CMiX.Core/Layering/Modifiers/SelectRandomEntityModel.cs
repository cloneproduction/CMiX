// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Modifiers;

namespace CMiX.Core.Layering.Modifiers
{
    public class SelectRandomEntityModel : IControlModel, IModifierModel
    {
        public SelectRandomEntityModel()
        {
            ID = Guid.NewGuid();
            Visible = new GenericValueModel<bool>(true);
            Easing = new EasingModel();
            BeatModifier = new BeatModifierModel();
        }

        public Guid ID { get; set; }
        public GenericValueModel<bool> Visible { get; set; }
        public EasingModel Easing { get; set; }
        public BeatModifierModel BeatModifier { get; set; }
    }
}
