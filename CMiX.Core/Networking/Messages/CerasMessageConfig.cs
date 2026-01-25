// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Ceras;

namespace CMiX.Core.Networking.Messages
{
    public static class CerasMessageConfiguration
    {
        public static void Configure(SerializerConfig config)
        {
            config.ConfigType<IControlModel>().ConstructByUninitialized();
            config.ConfigType<MessageAddItem>().ConstructByUninitialized();
            config.ConfigType<MessageMoveItem>().ConstructByUninitialized();
            config.ConfigType<MessageRemoveItem>().ConstructByUninitialized();
            config.ConfigType<MessageReplaceItem>().ConstructByUninitialized();
            config.ConfigType<MessageRemoveSelectedItem>().ConstructByUninitialized();
            config.ConfigType<MessageSelectedItemChanged>().ConstructByUninitialized();
            config.ConfigType<MessageOnClick>().ConstructByUninitialized();
            config.ConfigType<MessageValueChanged>().ConstructByUninitialized();
        }
    }
}
