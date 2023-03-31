// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.BaseControl;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Texturing.Filters;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Sources.VideoIn
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
            var videoInModel = new VideoInModel();

            videoInModel.ID = ID;
            videoInModel.SizeX = (IntegerValueModel)SizeX.GetModel();
            videoInModel.SizeY = (IntegerValueModel)SizeY.GetModel();

            return videoInModel;
        }
    }
}
