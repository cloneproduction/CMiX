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
            ID = Guid.NewGuid();

            SeekFrame = new IntegerValueModel();
            DoSeek = new ButtonModel();
            PlayModel = new BooleanValueModel();
            PlayModel.Value = true;
            Resolution = new Integer2Model(0, 0);
            Asset = new GenericValueModel<Asset>(null);
        }
        public Guid ID { get; set; }

        public ButtonModel DoSeek { get; set; }
        public IntegerValueModel SeekFrame { get; set; }
        public BooleanValueModel PlayModel { get; set; }
        public GenericValueModel<Asset> Asset { get; set; }
        public Integer2Model Resolution { get; internal set; }
    }
}
