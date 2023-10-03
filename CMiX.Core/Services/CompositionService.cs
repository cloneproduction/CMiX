// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.Components;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefab;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Cameras.Modifiers;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Rendering.Lights.Modifiers;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Transformation.Modifiers;

namespace CMiX.Core.Services
{
    public class CompositionService : IService
    {
        public CompositionService()
        {
            ProjectRepository = new PrefabRepository();
            CompositionRepository = new PrefabRepository();

            MasterBeat = new MasterBeat();
            PrefabFactory = new PrefabFactory(this);

            ProjectRepository = new PrefabRepository();
            CompositionRepository = new PrefabRepository();
            LayerRepository = new PrefabRepository();
            EntityRepository = new PrefabRepository();

            Repositories = new List<PrefabRepository>();
            Repositories.Add(ProjectRepository);
            Repositories.Add(CompositionRepository);
            Repositories.Add(LayerRepository);
            Repositories.Add(EntityRepository);

        }

        public List<PrefabRepository> Repositories { get; set; }
        public PrefabRepository ProjectRepository { get; set; }
        public PrefabRepository CompositionRepository { get; set; }
        public PrefabRepository LayerRepository { get; set; }
        public PrefabRepository EntityRepository { get; set; }


        public MasterBeat MasterBeat { get; set; }
        public PrefabFactory PrefabFactory { get; set; }

        public PrefabRepository GetPrefabRepository(Type ownerType)
        {
            if(ownerType == typeof(Project))
                return ProjectRepository;

            if(ownerType == typeof(Layer))
                return LayerRepository;

            if(ownerType == typeof(Entity))
                return EntityRepository;

            return null;
        }

        public ModifierFactory GetFactory(Type type)
        {
            if (type == typeof(ITextureModifier))
                return new TextureModifierFactory();

            if (type == typeof(Entity))
                return new EntityModifierFactory();

            if (type == typeof(Camera))
                return new CameraModifierFactory();

            if (type == typeof(LightEntity))
                return new LightModifierFactory();

            return null;
        }
    }
}
