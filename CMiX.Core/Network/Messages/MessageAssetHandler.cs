// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Assets;

namespace CMiX.Core.Network.Messages
{
    public class MessageAssetHandler : IMessageHandler
    {
        public MessageAssetHandler()
        {

        }
        public bool Handle(IControl control, IMessage message)
        {
            var assetSelector = control as IAssetSelector;
            var messageAsset = message as MessageAsset;

            if (assetSelector is ImageSelector imageSelector)
            {
                var asset = new AssetImage(messageAsset.AssetModel);
                imageSelector.SelectedAsset = asset;
                return true;
            }

            if (assetSelector is VideoSelector videoSelector)
            {
                var asset = new AssetVideo(messageAsset.AssetModel);
                videoSelector.SelectedAsset = asset;
                return true;
            }

            return false;
        }
    }
}
