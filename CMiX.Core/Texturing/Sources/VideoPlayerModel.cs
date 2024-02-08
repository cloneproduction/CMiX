// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.ViewModels.Assets;

namespace CMiX.Core.Texturing.Sources
{
    public class VideoPlayerModel : IControlModel
    {
        public VideoPlayerModel()
        {
            SeekFrame = new GenericValueModel<int>();
            DoSeek = new ButtonModel();
            Play = new GenericValueModel<bool>(true);
            Resolution = new Integer2Model(0, 0);
            Asset = new GenericValueModel<Asset>(null);
        }
        public Guid ID { get; set; } = Guid.NewGuid();

        public ButtonModel DoSeek { get; set; }
        public GenericValueModel<int> SeekFrame { get; set; }
        public GenericValueModel<bool> Play { get; set; }
        public GenericValueModel<Asset> Asset { get; set; }
        public Integer2Model Resolution { get; internal set; }
    }
}
