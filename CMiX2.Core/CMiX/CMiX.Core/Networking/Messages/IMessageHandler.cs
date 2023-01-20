// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Prefab;

namespace CMiX.Core.Networking.Messages
{
    public interface IMessageHandler
    {
        bool Handle(IControl control, IMessage message);
    }
}
