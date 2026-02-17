// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using Mapster;
using MapsterMapper;

namespace CMiX.Core.Mapping
{
    public class Mapper
    {
        public Mapper(IServiceProvider services, IMapper mapper)
        {
            ServiceProvider = services;
            DtoMapper = mapper;
        }

        public IMapper DtoMapper { get; set; }
        public IServiceProvider ServiceProvider { get; set; }

        public IControl MapToViewModel(IControl control, IControlModel controlModel)
        {
            var c = (IControl)TypeAdapter.Adapt(controlModel, control, controlModel.GetType(), control.GetType());
            return c;
        }

        public IControlModel MapToModel(IControl control)
        {
            Type modelType;

            if (control.GetType().IsGenericType && control.GetType().GetGenericTypeDefinition() == typeof(GenericValue<>))
            {
                var genericArg = control.GetType().GetGenericArguments()[0];
                modelType = typeof(GenericValueModel<>).MakeGenericType(genericArg);
            }
            else
            {
                var modelTypeName = control.GetType().FullName + "Model";
                modelType = control.GetType().Assembly.GetType(modelTypeName)
                            ?? throw new InvalidOperationException($"No model type found for {control.GetType().Name}");
            }

            return(IControlModel)TypeAdapter.Adapt(control, control.GetType(), modelType);
        }
    }
}
