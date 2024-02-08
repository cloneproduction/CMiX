// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Animations
{
    public class BeatModifierModel : IControlModel
    {
        public BeatModifierModel()
        {
            ID = Guid.NewGuid();
            ChanceToHit = new GenericValueModel<float>(100);
            BeatIndex = new GenericValueModel<int>(0);
        }

        public Guid ID { get; set; }
        public GenericValueModel<int> BeatIndex { get; set; }
        public GenericValueModel<float> ChanceToHit { get; set; }
    }
}
