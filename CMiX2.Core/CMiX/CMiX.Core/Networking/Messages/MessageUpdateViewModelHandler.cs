// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Diagnostics;
using AutoMapper;
using CMiX.Core.Mapper;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels;
using CMiX.Core.Presentation.ViewModels.Service;

namespace CMiX.Core.Networking.Messages
{
    public class MessageUpdateViewModelHandler : IMessageHandler
    {
        public MessageUpdateViewModelHandler()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile(new MappingProfile());
            });

            Mapper = config.CreateMapper();
        }

        private IMapper Mapper { get; set; }

        public bool Handle(IControl control, IMessage message)
        {
            if (message is MessageUpdateViewModel messageUpdateViewModel)
            {
                //if(messageUpdateViewModel.Model is FloatValueModel floatValueModel)
                //{
                //    control = Mapper.Map<FloatValue>(floatValueModel);
                //    Debug.WriteLine(((FloatValue)control).Value);
                //    return true;
                //}

                control.SetViewModel(messageUpdateViewModel.Model);
                return true;
            }

            return false;
        }
    }
}
