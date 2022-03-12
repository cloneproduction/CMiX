// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Network.Messages
{
    public class MessageRemoveTextureFilter : IMessage
    {
        public MessageRemoveTextureFilter()
        {

        }

        public MessageRemoveTextureFilter(Guid ParentID, ITextureFilter textureFilter)
        {
            this.ID = ParentID;
            this.TextureFilterModel = textureFilter.GetModel();
        }

        public Guid ID { get; set; }
        public IModel TextureFilterModel { get; set; }
    }
}
