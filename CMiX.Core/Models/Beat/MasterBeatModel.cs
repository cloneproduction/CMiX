// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Models.Beat
{
    public class MasterBeatModel : BeatModel, IPrefabModel
    {
        public MasterBeatModel()
        {
            this.ID = Guid.NewGuid();
            ResyncModel = new ResyncModel();
            Pause = new ToggleButtonModel(false);
        }


        public ResyncModel ResyncModel { get; set; }
        public int BeatIndex { get; set; }
        public ToggleButtonModel Pause { get; internal set; }
    }
}
