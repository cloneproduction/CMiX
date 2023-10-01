// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Animations;
using CMiX.Core.Compositing;
using CMiX.Core.Modifiers;
using CMiX.Core.Prefab;
using CMiX.Core.Rendering.Cameras;
using CMiX.Core.Rendering.Cameras.Modifiers;
using CMiX.Core.Texturing;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Transformation.Modifiers;

namespace CMiX.Core.Services
{
    public class CompositionService : IService
    {
        public CompositionService()
        {
            PrefabRepository = new PrefabRepository();
            MasterBeat = new MasterBeat();
            PrefabFactory = new PrefabFactory(this);
        }

        public PrefabRepository PrefabRepository { get; set; }
        public MasterBeat MasterBeat { get; set; }
        public PrefabFactory PrefabFactory { get; set; }


        public ModifierFactory GetFactory(Type type)
        {
            if (type == typeof(ITextureModifier))
                return new TextureModifierFactory();

            if (type == typeof(Entity))
                return new EntityModifierFactory();

            if (type == typeof(Camera))
                return new CameraModifierFactory();

            return null;
        }
    }
}
