// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace CMiX.Core.Prefabs.Messages
{
    public class MessageMovePrefab : IMessagePrefab
    {
        public MessageMovePrefab()
        {

        }

        public MessageMovePrefab(Guid id, int oldIndex, int newIndex)
        {
            ID = id;
            OldIndex = oldIndex;
            NewIndex = newIndex;
        }

        public Guid ID { get; set; }
        public int OldIndex { get; set; }
        public int NewIndex { get; set; }
    }
}
