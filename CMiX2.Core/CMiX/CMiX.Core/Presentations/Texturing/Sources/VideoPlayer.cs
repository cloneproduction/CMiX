// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControl;
using CMiX.Core.Presentations.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.Texturing.Sources
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


        public IModel GetModel()
        {
            var videoPlayerModel = new VideoPlayerModel();

            videoPlayerModel.ID = ID;
            videoPlayerModel.SeekFrame = (IntegerValueModel)SeekFrame.GetModel();
            videoPlayerModel.PlayModel = (BooleanValueModel)Play.GetModel();
            videoPlayerModel.DoSeek = (ButtonModel)DoSeek.GetModel();
            return videoPlayerModel;
        }
    }
}
