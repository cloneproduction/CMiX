// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Core
{
    public class ModifierConfigurator : ControlConfigurator
    {
        public ModifierConfigurator(IMapperConfigurationExpression configurationExpression, IServiceCollection serviceCollection) : base(configurationExpression, serviceCollection)
        {
            ConfigurationExpression = configurationExpression;
            ServiceCollection = serviceCollection;
        }

        public Dictionary<Type, Func<IControl>> CreateControlFactory(IMapper mapper, ControlPairProfile controlPairProfile)
        {
            var factories = new Dictionary<Type, Func<IControl>>();
            foreach (var pair in controlPairProfile.ControlModelPairs)
            {
                factories.Add(pair.Item1, new Func<IControl>(() => mapper.Map((IControlModel)Activator.CreateInstance(pair.Item2), (IControl)Activator.CreateInstance(pair.Item1))));
            }
            return factories;
        }

        public Dictionary<Type, Func<IControlModel, IControl>> CreateModelToControlFactory(IMapper mapper, ControlPairProfile controlPairProfile)
        {
            var factories = new Dictionary<Type, Func<IControlModel, IControl>>();
            foreach (var pair in controlPairProfile.ControlModelPairs)
            {
                factories.Add(pair.Item2, new Func<IControlModel, IControl>(controlModel => mapper.Map(controlModel, (IControl)Activator.CreateInstance(pair.Item1))));
            }
            return factories;
        }
    }
}
