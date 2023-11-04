// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Numerics;
using CMiX.Core.BaseControls;
using CMiX.Core.ViewModels;
using CMiX.Core.ViewModels.Assets;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Sources
{
    public class VideoPlayer : ObservableObject, ITextureSource
    {
        public VideoPlayer(Integer2 resolution, IntegerValue seekFrame, BooleanValue play, Button doSeek, GenericValue<Asset> asset)
        {
            Resolution = resolution;// new Integer2(0, 0);
            SeekFrame = seekFrame; // new IntegerValue(0);
            Play = play; // new BooleanValue(true);
            DoSeek = doSeek; // new Button();
            Asset = asset; // new GenericValue<Asset>(null);
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public Button DoSeek { get; set; }
        public IntegerValue SeekFrame { get; set; }
        public BooleanValue Play { get; set; }
        public GenericValue<Asset> Asset { get; set; }
        public Integer2 Resolution { get; set; }
    }
}
