// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Animations
{
    public class MasterBeatModel : IControlModel
    {
        public MasterBeatModel()
        {
            ID = Guid.NewGuid();
            Resync = new ButtonModel();
            Pause = new BooleanValueModel(false);
            Index = new IntegerValueModel(0);
            BeatIndex = new IntegerValueModel(0);
            Period = new FloatValueModel(1000);
        }

        public Guid ID { get; set; }
        public ButtonModel Resync { get; set; }
        public BooleanValueModel Pause { get; set; }
        public IntegerValueModel Index { get; set; }
        public IntegerValueModel BeatIndex { get; set; }
        public FloatValueModel Period { get; set; }
    }
}
