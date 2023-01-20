// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentation.ViewModels
{
    public class VideoIn : ObservableRecipient, IControl
    {
        public VideoIn(VideoInModel videoInModel)
        {
            ID = videoInModel.ID;
            SizeX = new IntegerValue(videoInModel.SizeX);
            SizeY = new IntegerValue(videoInModel.SizeY);
        }

        public Guid ID { get; set; }
        public IntegerValue SizeX { get; set; }
        public IntegerValue SizeY { get; set; }

        public IModel GetModel()
        {
            VideoInModel videoInModel = new VideoInModel();

            videoInModel.ID = ID;
            videoInModel.SizeX = (IntegerValueModel)this.SizeX.GetModel();
            videoInModel.SizeY = (IntegerValueModel)this.SizeY.GetModel();

            return videoInModel;
        }

        public void SetViewModel(IModel model)
        {
            VideoInModel videoInModel = model as VideoInModel;
            this.ID = videoInModel.ID;
            this.SizeX.SetViewModel(videoInModel.SizeX);
            this.SizeY.SetViewModel(videoInModel.SizeY);
        }
    }
}
