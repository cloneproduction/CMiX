// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControl;
using CMiX.Core.Presentations.Prefabs;

namespace CMiX.Core.Presentations.Beat
{
    public class MasterBeatModel : BeatModel, IPrefabModel
    {
        public MasterBeatModel()
        {
            ID = Guid.NewGuid();
            ResyncModel = new ResyncModel();
            Pause = new BooleanValueModel(false);
        }


        public ResyncModel ResyncModel { get; set; }
        public int BeatIndex { get; set; }
        public BooleanValueModel Pause { get; internal set; }
    }
}
