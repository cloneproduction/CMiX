// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;

namespace CMiX.Core.Prefabs
{
    public class ControlFactory
    {
        public ControlFactory(IMapper mapper)
        {
            Mapper = mapper;
        }

        internal IMapper Mapper { get; set; }

        public IControl Create(Type type) 
        {
            var controlModel = (IControlModel)Activator.CreateInstance(type);
            return Mapper.Map<IControlModel, IControl>(controlModel);
        }

        public IControl Create(IControlModel controlModel)
        {
            return Mapper.Map<IControlModel, IControl>(controlModel);
        }
    }
}
