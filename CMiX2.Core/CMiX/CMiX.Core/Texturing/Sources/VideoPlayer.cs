// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Sources
{
    public class VideoPlayer : ObservableRecipient, IControl
    {
        public VideoPlayer(VideoPlayerModel videoPlayerModel)
        {
            ID = videoPlayerModel.ID;

            SeekFrame = new IntegerValue(videoPlayerModel.SeekFrame);
            Play = new BooleanValue(videoPlayerModel.PlayModel);
            DoSeek = new Button(videoPlayerModel.DoSeek);
        }

        public Guid ID { get; set; }
        public Button DoSeek { get; set; }
        public IntegerValue SeekFrame { get; set; }
        public BooleanValue Play { get; set; }
    }
}
