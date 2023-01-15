// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class VideoPlayer : ObservableRecipient, IControl
    {
        public VideoPlayer(VideoPlayerModel videoPlayerModel)
        {
            this.ID = videoPlayerModel.ID;

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
            VideoPlayerModel videoPlayerModel = new VideoPlayerModel();

            videoPlayerModel.ID = ID;
            videoPlayerModel.SeekFrame = (IntegerValueModel)SeekFrame.GetModel();
            videoPlayerModel.PlayModel = (BooleanValueModel)Play.GetModel();
            videoPlayerModel.DoSeek = (ButtonModel)DoSeek.GetModel();
            return videoPlayerModel;
        }

        public void SetViewModel(IModel model)
        {
            VideoPlayerModel videoPlayerModel = model as VideoPlayerModel;
            this.ID = videoPlayerModel.ID;
            this.SeekFrame.SetViewModel(videoPlayerModel.SeekFrame);
            this.Play.SetViewModel(videoPlayerModel.PlayModel);
            this.DoSeek.SetViewModel(videoPlayerModel.DoSeek);
        }
    }
}
