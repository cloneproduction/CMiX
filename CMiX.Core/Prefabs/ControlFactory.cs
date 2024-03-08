// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;

namespace CMiX.Core.Prefabs
{
    public class ControlFactory
    {
        public ControlFactory(ControlRepository controlRepository, IMapper mapper)
        {
            ControlRepository = controlRepository;
            Mapper = mapper;
        }

        internal IMapper Mapper { get; set; }
        internal ControlRepository ControlRepository { get; set; }

        public IControl Create(Type type) 
        {
            var control = Mapper.Map<IControlModel, IControl>((IControlModel)Activator.CreateInstance(type));
            ControlRepository.AddControl(control);
            return control;
        }

        public IControl Create(IControlModel controlModel)
        {
            var control = Mapper.Map<IControlModel, IControl>(controlModel);
            ControlRepository.AddControl(control);
            return control;
        }

        public IControl GetPrefab(Guid id)
        {
            return ControlRepository.GetPrefab(id);
        }
    }
}
