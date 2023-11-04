// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System.Windows.Media;
using AutoMapper;
using CMiX.Core.BaseControls;
using CMiX.Core.Materials;
using CMiX.Core.Modifiers;
using CMiX.Core.Networking;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Services;
using CMiX.Core.Texturing;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sampling;
using CMiX.Core.Transformation.Modifiers;
using CMiX.Core.ViewModels;

namespace CMiX.Core.Compositing
{
    public class EntityFactory : IPrefabFactory
    {
        public EntityFactory(PrefabRepositories prefabRepositories, IMapper mapper)
        {
            PrefabRepositories = prefabRepositories;
            LayerRepository = prefabRepositories.LayerRepository;
            Mapper = mapper;
        }

        PrefabRepositories PrefabRepositories { get; set; }
        PrefabRepository LayerRepository { get; set; }
 
        IMapper Mapper { get; set; }

        public IPrefab GetPrefab(Guid id)
        {
            return LayerRepository.GetPrefab(id);
        }

        public bool AppliesTo(Type type)
        {
            return (typeof(Entity).Equals(type) || typeof(EntityModel).Equals(type));
        }


        Material CreateMaterial(ControlMessenger controlMessenger)
        {
            var textureFactory = new TextureFactory(PrefabRepositories, Mapper);

            var prefabFactory = new PrefabFactory();
            prefabFactory.RegisterFactory(textureFactory);

            var visible = new BooleanValue(false, controlMessenger);
            var samplerState = new SamplerState();

            var transformTexture = new TransformTexture(visible, samplerState);

            var prefabManagerMessenger = new PrefabManagerMessenger(Mapper);
            var maskManagerBase = new PrefabManagerBase(Guid.NewGuid(), prefabFactory, prefabManagerMessenger);

            var samplerState = new SamplerState();
            var maskTexture = new MaskTexture(maskManagerBase, transformTexture, samplerState);

            var diffuseTexture = new DiffuseTexture(new PrefabManagerBase(Guid.NewGuid(), prefabFactory));

            var pipeline = new GenericValue<PipelineType>(PipelineType.Constant, controlMessenger);
            var cullMode = new GenericValue<CullModeType>(CullModeType.Back, controlMessenger);
            var transparency = new GenericValue<TransparencyType>(TransparencyType.CutOff, controlMessenger);

            var metalness = new FloatValue(0.5f, controlMessenger);
            var specularity = new FloatValue(0.5f, controlMessenger);
            var glossiness = new FloatValue(0.5f, controlMessenger);
            var alpha = new FloatValue(1.0f, controlMessenger);
            var isShadowCaster = new BooleanValue(false, controlMessenger);

            var baseColor = new ColorValue(Color.FromArgb(255, 255, 255, 255), controlMessenger);

            var materialModel = new MaterialModel();
            var material = Mapper.Map<Material>(materialModel);
            return material; // new Material(diffuseTexture, maskTexture, pipeline, cullMode, transparency, metalness, specularity, glossiness, alpha, isShadowCaster, baseColor);
        }

        Mesh CreateMesh(ControlMessenger controlMessenger)
        {
            var meshTypeSelector = new GenericValue<MeshType>(MeshType.Plane, controlMessenger);
            var scale = new Vector3(1.0f, 1.0f, 1.0f);
            var offset = new Vector3();
            var radius = new FloatValue(1.0f, controlMessenger);
            var height = new FloatValue(1.0f, controlMessenger);
            var thickness = new FloatValue(1.0f, controlMessenger);
            var tessellation = new IntegerValue(16, controlMessenger);
            var tessellationXY = new Integer2(16, 16);
            var generateBackFace = new BooleanValue(false, controlMessenger);
            var visibility = new BooleanValue(false, controlMessenger);

            return new Mesh(meshTypeSelector, scale, offset, radius, height, thickness, tessellation, tessellationXY, generateBackFace, visibility);
        }


        public IPrefab CreatePrefab(PrefabService prefabService)
        {

            var controlMessenger = new ControlMessenger(Mapper);

            var material = CreateMaterial(controlMessenger);
            var mesh = CreateMesh(controlMessenger);

            var modifierManager = new ModifierManager(new EntityModifierFactory());
            var entity = new Entity(prefabService, mesh, material, modifierManager);

            LayerRepository.AddPrefab(entity);

            return entity;
        }

        public IPrefab CreatePrefab(PrefabService prefabService, IPrefabModel prefabModel)
        {
            var entity = Mapper.Map(prefabModel, CreatePrefab(prefabService));

            LayerRepository.AddPrefab(entity);

            return entity;
        }
    }
}
