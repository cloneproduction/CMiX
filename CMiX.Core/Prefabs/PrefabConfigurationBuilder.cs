// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Compositing;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Services;
using CMiX.Core.Texturing;
using Microsoft.Extensions.DependencyInjection;

namespace CMiX.Core.Prefabs
{
    public class PrefabConfigurationBuilder
    {
        public PrefabConfigurationBuilder(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<PrefabService>();

            serviceCollection.AddSingleton<ManagerMessenger>();
            serviceCollection.AddTransient<ManagerData>();
            serviceCollection.AddTransient<ReorderablePrefabManager>();
            serviceCollection.AddTransient<ManagerReorderService>();


            serviceCollection.AddSingleton(x => new Project(new PrefabManager(new ManagerData(Guid.Parse("11223344-5566-7788-99AA-BBCCDDEEFF00")), x.GetRequiredService<ControlFactory>(), x.GetRequiredService<ManagerMessenger>())));

            serviceCollection.AddTransient<Composition>();

            serviceCollection.AddTransient<Layer>();
            serviceCollection.AddTransient<LayerSettings>();
            serviceCollection.AddTransient<LayerMaskService>();

            serviceCollection.AddTransient<EmptyPrefab>();
            serviceCollection.AddTransient<EmptyPrefabService>();

            serviceCollection.AddTransient<Entity>();
            serviceCollection.AddTransient<Texture>();
            serviceCollection.AddTransient<LightEntity>();


            serviceCollection.AddTransient<ControlRepository>();
            serviceCollection.AddSingleton(ctx =>
            {
                var projectRepository = ctx.GetRequiredService<ControlRepository>();
                var compositionRepository = ctx.GetRequiredService<ControlRepository>();
                var layerRepository = ctx.GetRequiredService<ControlRepository>();
                var entityRepository = ctx.GetRequiredService<ControlRepository>();

                var repositories = new Dictionary<Type, ControlRepository>()
                {
                    [typeof(Composition)] = projectRepository,
                    [typeof(Layer)] = compositionRepository,
                    [typeof(Entity)] = layerRepository,
                    [typeof(LightEntity)] = layerRepository,
                    [typeof(Camera)] = layerRepository,
                    [typeof(Texture)] = entityRepository
                };

                return new PrefabRepositories(repositories);
            });
        }
    }
}
