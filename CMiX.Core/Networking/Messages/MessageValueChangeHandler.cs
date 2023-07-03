// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;

namespace CMiX.Core.Networking.Messages
{
    public class MessageValueChangeHandler : IMessageHandler
    {
        public MessageValueChangeHandler(IMapper mapper)
        {
            Mapper = mapper;
        }

        private IMapper Mapper { get; set; }

        public bool Handle(IControl control, IMessage message)
        {
            if (message is MessageValueChange messageUpdateViewModel)
            {
                Mapper.Map(messageUpdateViewModel.Model, control);
                return true;
            }

            return false;
        }
    }
}
