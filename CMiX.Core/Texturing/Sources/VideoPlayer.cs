// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.ViewModels;
using CMiX.Core.ViewModels.Assets;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Sources
{
    public class VideoPlayer : ObservableObject, ITextureSource
    {
        public VideoPlayer(VideoPlayerModel videoPlayerModel)
        {
            ID = videoPlayerModel.ID;
            Resolution = new Integer2(videoPlayerModel.Resolution);
            SeekFrame = new IntegerValue(videoPlayerModel.SeekFrame);
            Play = new BooleanValue(videoPlayerModel.PlayModel);
            DoSeek = new Button(videoPlayerModel.DoSeek);
            Asset = new GenericValue<Asset>(videoPlayerModel.Asset);
        }

        public Guid ID { get; set; }
        public Button DoSeek { get; set; }
        public IntegerValue SeekFrame { get; set; }
        public BooleanValue Play { get; set; }
        public GenericValue<Asset> Asset { get; set; }
        public Integer2 Resolution { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
