// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Prefabs.Messages
{
    public class MessageSelectedItemChanged : IMessageManager
    {
        public MessageSelectedItemChanged()
        {

        }

        public MessageSelectedItemChanged(Guid id, IControlModel control, int index)
        {
            ID = id;
            Control = control;
            Index = index;
        }

        public Guid ID { get; set; }
        public IControlModel Control { get; set; }
        public int Index { get; set; }
    }
}
