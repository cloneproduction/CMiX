// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Presentations.Texturing.Sources
{
    public class VideoIn : ObservableRecipient, IControl
    {
        public VideoIn(VideoInModel videoInModel, CompositionService compositionService)
        {
            ID = videoInModel.ID;
            SizeX = new IntegerValue(videoInModel.SizeX, compositionService);
            SizeY = new IntegerValue(videoInModel.SizeY, compositionService);
        }

        public Guid ID { get; set; }
        public IntegerValue SizeX { get; set; }
        public IntegerValue SizeY { get; set; }
    }
}
