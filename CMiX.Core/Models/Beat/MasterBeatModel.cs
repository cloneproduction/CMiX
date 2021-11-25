// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Models.Beat
{
    public class MasterBeatModel : BeatModel, IModel
    {
        public MasterBeatModel()
        {
            this.ID = Guid.NewGuid();
            ResyncModel = new ResyncModel();
        }


        public ResyncModel ResyncModel { get; set; }
    }
}
