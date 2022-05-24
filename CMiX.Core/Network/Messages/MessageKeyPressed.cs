// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Input;

namespace CMiX.Core.Network.Messages
{
    public class MessageKeyPressed
    {
        public MessageKeyPressed(Key key)
        {
            Key = key;
        }

        public Key Key { get; set; }
    }
}
