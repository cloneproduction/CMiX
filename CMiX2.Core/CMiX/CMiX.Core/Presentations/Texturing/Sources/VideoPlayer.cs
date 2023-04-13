// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControl;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.ViewModels;
using CMiX.Core.Presentations.ViewModels.BaseControl;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.Texturing.Sources
{
    public class VideoPlayer : ObservableRecipient, IControl
    {
        public VideoPlayer(VideoPlayerModel videoPlayerModel, CompositionService compositionService)
        {
            ID = videoPlayerModel.ID;

            SeekFrame = new IntegerValue(videoPlayerModel.SeekFrame, compositionService);
            Play = new BooleanValue(videoPlayerModel.PlayModel, compositionService);
            DoSeek = new Button(videoPlayerModel.DoSeek, compositionService);
        }

        public Guid ID { get; set; }
        public Button DoSeek { get; set; }
        public IntegerValue SeekFrame { get; set; }
        public BooleanValue Play { get; set; }
    }
}
