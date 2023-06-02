// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.ViewModels;
using CMiX.Core.ViewModels.Assets;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CMiX.Core.Texturing.Sources
{
    public class Image : ObservableObject, ITextureSource
    {
        public Image(ImageModel imageModel)
        {
            ID = imageModel.ID;
            Resolution = new Integer2(imageModel.Resolution);
            Asset = new GenericValue<Asset>(imageModel.Asset);
        }

        public Integer2 Resolution { get; set; }
        public Guid ID { get; set; }
        public GenericValue<Asset> Asset { get; set; }
    }
}
