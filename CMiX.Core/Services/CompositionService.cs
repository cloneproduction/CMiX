// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.BaseControls;
using CMiX.Core.Components;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefab;
using CMiX.Core.Prefab.Managers;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Cameras.Modifiers;
using CMiX.Core.Rendering.Lights;
using CMiX.Core.Rendering.Lights.Modifiers;
using CMiX.Core.Texturing;
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
        public MasterBeat MasterBeat { get; set; } = new MasterBeat();
        public PrefabFactory PrefabFactory { get; set; }

        public PrefabRepository GetPrefabRepository<T>()
        {
            if(typeof(T) == typeof(Project))
                return ProjectRepository;

            if (typeof(T) == typeof(Composition))
                return CompositionRepository;

            if (typeof(T) == typeof(Layer))
                return LayerRepository;

            if(typeof(T) == typeof(Entity))
                return EntityRepository;

            if (typeof(T) == typeof(ITexture))
                return EntityRepository;

            return null;
        }

        public ModifierFactory GetFactory<T>()
        {
            if (typeof(T) == typeof(ITextureModifier))
                return new TextureModifierFactory();

            if (typeof(T) == typeof(Entity))
                return new EntityModifierFactory();

            if (typeof(T) == typeof(Camera))
                return new CameraModifierFactory();

            if (typeof(T) == typeof(LightEntity))
                return new LightModifierFactory();

            return null;
        }

        public ModifierManager GetModifierManager<T>()
        {
            var factory = GetFactory<T>();
            return new ModifierManager(factory);
        }

        public PrefabManagerSlot GetPrefabManagerSlot<T>()
        {
            if(typeof(T) == typeof(Layer))
                return new PrefabManagerSlot(this.GetPrefabRepository<T>(), this.PrefabFactory);

            return null;
        }

        public PrefabManager GetPrefabManagerDraggable<T>()
        {
            if (typeof(T) == typeof(Composition))
                return new DraggablePrefabManager(this.GetPrefabRepository<T>(), this.PrefabFactory);

            return null;
        }

        public Guid CompositionManagerID { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");

        public PrefabManagerBase GetPrefabManager<T>()
        {
            if (typeof(T) == typeof(Project))
                return new PrefabManagerBase(CompositionManagerID, this.GetPrefabRepository<T>(), this.PrefabFactory);

            if(typeof(T) == typeof(ITexture))
                return new PrefabManagerBase(this.GetPrefabRepository<T>(), this.PrefabFactory);

            return null;
        }

        internal FloatValue GetFloatControl(float f)
        {
            return new FloatValue(f);
        }

        internal BooleanValue GetBooleanControl(bool b) 
        {  
            return new BooleanValue(b); 
        }
    }
}
