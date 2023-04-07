// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControl;

namespace CMiX.Core.Presentations.Beat
{
    public class BeatModifierModel : BeatModel, IModel
    {
        public BeatModifierModel()
        {
            ID = Guid.NewGuid();
            ChanceToHit = new FloatValueModel { Value = 100.0f };
            BeatIndex = 0;
        }

        public int BeatIndex { get; set; }
        public FloatValueModel ChanceToHit { get; set; }
    }
}
