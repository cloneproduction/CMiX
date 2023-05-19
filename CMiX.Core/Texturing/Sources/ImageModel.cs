// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.ViewModels.Assets;

namespace CMiX.Core.Texturing.Sources
{
    public class ImageModel : IModel
    {
        public ImageModel()
        {
            ID = Guid.NewGuid();
            Resolution = new Integer2Model(0, 0);
            Asset = new GenericValueModel<Asset>(null);
        }

        public Integer2Model Resolution { get; set; }
        public Guid ID { get; set; }
        public GenericValueModel<Asset> Asset { get; set; }

        public void Dispose()
        {

        }
    }
}
