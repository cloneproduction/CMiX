// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels;

namespace CMiX.Core.Network.Messages
{
    public class MessageAddTextureFilter : IMessage
    {
        public MessageAddTextureFilter()
        {

        }

        public MessageAddTextureFilter(Guid parentID, ITextureFilter textureFilter)
        {
            ID = parentID;
            TextureFilterModel = textureFilter.GetModel() as ITextureFilterModel;
        }


        public ITextureFilterModel TextureFilterModel { get; set; }
        public Guid ID { get; set; }
    }
}
