// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Prefabs.Messages
{
    public class MessageReplaceItem : IMessageManager
    {
        public MessageReplaceItem()
        {

        }

        public MessageReplaceItem(Guid id, IControlModel controlModel, int index)
        {
            ID = id;
            ControlModel = controlModel;
            Index = index;
        }

        public Guid ID { get; set; }
        public IControlModel ControlModel { get; private set; }
        public int Index { get; private set; }
    }
}
