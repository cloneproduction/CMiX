// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;
using System;

namespace CMiX.Core.Models
{
    public class BeatModifierModel : BeatModel, IModel
    {
        public BeatModifierModel()
        {
            this.ID = Guid.NewGuid();
            ChanceToHit = new SliderModel { Amount = 100.0 };
            BeatIndex = 0;
        }

        public int BeatIndex { get; set; }
        public SliderModel ChanceToHit { get; set; }
    }
}
