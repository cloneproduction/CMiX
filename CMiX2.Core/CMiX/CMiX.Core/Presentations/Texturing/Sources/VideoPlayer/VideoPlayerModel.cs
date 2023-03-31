// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.BaseControl;

namespace CMiX.Core.Texturing.Sources.VideoPlayer
{
    public class VideoPlayerModel : IModel
    {
        public VideoPlayerModel()
        {
            ID = Guid.NewGuid();
            SeekFrame = new IntegerValueModel();
            DoSeek = new ButtonModel();
            PlayModel = new BooleanValueModel();
            PlayModel.Value = true;
        }
        public Guid ID { get; set; }

        public ButtonModel DoSeek { get; set; }
        public IntegerValueModel SeekFrame { get; set; }
        public BooleanValueModel PlayModel { get; set; }
    }
}
