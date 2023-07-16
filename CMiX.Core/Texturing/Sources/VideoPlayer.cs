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
        public VideoPlayer()
        {
            Resolution = new Integer2();
            SeekFrame = new IntegerValue();
            Play = new BooleanValue();
            DoSeek = new Button();
            Asset = new GenericValue<Asset>();
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public Button DoSeek { get; set; }
        public IntegerValue SeekFrame { get; set; }
        public BooleanValue Play { get; set; }
        public GenericValue<Asset> Asset { get; set; }
        public Integer2 Resolution { get; set; }
    }
}
