// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControl;
using CMiX.Core.Presentations.ViewModels.BaseControl;

namespace CMiX.Core.Beat
{
    public class BeatModifierModel : IModel
    {
        public BeatModifierModel()
        {
            ID = Guid.NewGuid();
            ChanceToHit = new FloatValueModel(100);
            BeatIndex = new IntegerValueModel(0);
        }

        public Guid ID { get; set; }
        public IntegerValueModel BeatIndex { get; set; }
        public FloatValueModel ChanceToHit { get; set; }
    }
}
