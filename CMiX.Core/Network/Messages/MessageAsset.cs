// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Assets;

namespace CMiX.Core.Network.Messages
{
    public class MessageAsset : IMessage
    {
        public MessageAsset()
        {

        }

        public MessageAsset(IControl control, IAsset asset)
        {
            ID = control.ID;
            AssetModel = asset.GetModel() as IAssetModel;
        }

        public Guid ID { get; set; }
        public IAssetModel AssetModel { get; set; }

    }
}
