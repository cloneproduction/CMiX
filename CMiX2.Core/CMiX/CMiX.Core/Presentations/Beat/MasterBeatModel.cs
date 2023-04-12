// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControl;
using CMiX.Core.Presentations.Prefabs;

namespace CMiX.Core.Presentations.Beat
{
    public class MasterBeatModel : IPrefabModel
    {
        public MasterBeatModel()
        {
            ID = Guid.NewGuid();
            ResyncModel = new ResyncModel();
            Pause = new BooleanValueModel(false);
            Index = new IntegerValueModel(0);
            BeatIndex = new IntegerValueModel(0);
            Period = new FloatValueModel(1000);
        }

        public Guid ID { get; set; }
        public ResyncModel ResyncModel { get; set; }
        public BooleanValueModel Pause { get; set; }
        public IntegerValueModel Index { get; set; }
        public IntegerValueModel BeatIndex { get; set; }
        public FloatValueModel Period { get; set; }
    }
}
