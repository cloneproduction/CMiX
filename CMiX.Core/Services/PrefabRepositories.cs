// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;
using CMiX.Core.Texturing;

namespace CMiX.Core.Services
{
    public class PrefabRepositories : IService
    {
        public PrefabRepositories(Dictionary<Type, ControlRepository> repositories)
        {
            Repositories = repositories;
            EntityRepository = repositories.GetValueOrDefault(typeof(Texture));
            LayerRepository = repositories.GetValueOrDefault(typeof(Entity));
            ProjectRepository = repositories.GetValueOrDefault(typeof(Composition));
        }

        public ControlRepository EntityRepository { get; set; }
        public ControlRepository LayerRepository { get; set; }
        public ControlRepository ProjectRepository { get; set; }

        public Dictionary<Type, ControlRepository> Repositories { get; set; }

        public void AddControl(IControl control)
        {
            if(Repositories.TryGetValue(control.GetType(), out var repository))
                repository.AddControl(control);
        }

        public ControlRepository GetRepository(Type type)
        {
            return Repositories.TryGetValue(type, out var repository) ? repository : null;
        }
    }
}
