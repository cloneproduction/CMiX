// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Core
{
    public class ControlConfigurator
    {
        public ControlConfigurator(IMapperConfigurationExpression configurationExpression, IServiceCollection serviceCollection)
        {
            ConfigurationExpression = configurationExpression;
            ServiceCollection = serviceCollection;
        }

        internal IServiceCollection ServiceCollection { get; set; }
        internal IMapperConfigurationExpression ConfigurationExpression { get; set; }


        public void Register(ControlPairProfile controlConfiguration)
        {
            foreach (var item in controlConfiguration.ControlModelPairs)
            {
                var mappingProfile = new ControlMappingProfile(item.Item1, item.Item2);

                ConfigurationExpression.AddProfile(mappingProfile);
                ServiceCollection.AddTransient(item.Item1);
            }
        }
    }
}
