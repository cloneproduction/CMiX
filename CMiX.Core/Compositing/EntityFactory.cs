// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using AutoMapper;
using CMiX.Core.Assets.Materials;
using CMiX.Core.BaseControls;
using CMiX.Core.Materials;
using CMiX.Core.Networking;
using CMiX.Core.Prefabs;
using CMiX.Core.Prefabs.Managers;
using CMiX.Core.Prefabs.Messages;
using CMiX.Core.Texturing;
using CMiX.Core.Texturing.Filters;
using CMiX.Core.Texturing.Sampling;
using CMiX.Core.Transformation;
using CMiX.Core.ViewModels;

namespace CMiX.Core.Compositing
{
    public class EntityFactory : IPrefabFactory
    {
        public EntityFactory(PrefabRepository prefabRepository, IMapper mapper, Dictionary<Type, Func<IControl>> factories)
        {
            LayerRepository = prefabRepository;
            Mapper = mapper;
        }

        PrefabRepository LayerRepository { get; set; }
        Dictionary<Type, Func<IControl>> Factories { get; set; }

        IMapper Mapper { get; set; }

        public IPrefab GetPrefab(Guid id)
        {
            return LayerRepository.GetPrefab(id);
        }

        public bool AppliesTo(Type type)
        {
            return (typeof(Entity).Equals(type) || typeof(EntityModel).Equals(type));
        }

        T CreateControl<T>() where T : IControl
        {
            if (!Factories.TryGetValue(typeof(T), out var factory) || factory is null)
                throw new ArgumentOutOfRangeException(nameof(T), $"type '{typeof(T)}' is not registered");
            return (T)factory();
        }

        //MaterialSettings CreateMaterialSettings(ControlMessenger controlMessenger)
        //{
        //    var pipeline = new GenericValue<PipelineType>(controlMessenger);
        //    var cullMode = new GenericValue<CullModeType>(controlMessenger);
        //    var transparency = new GenericValue<TransparencyType>(controlMessenger);

        //    var metalness = new FloatValue(controlMessenger);
        //    var specularity = new FloatValue(controlMessenger);
        //    var glossiness = new FloatValue(controlMessenger);
        //    var alpha = new FloatValue(controlMessenger);
        //    var isShadowCaster = new BooleanValue(controlMessenger);

        //    var baseColor = new ColorValue(controlMessenger);

        //    return new MaterialSettings(pipeline, cullMode, transparency, metalness, specularity, glossiness, alpha, isShadowCaster, baseColor);
        //}

        //SamplerState CreateSamplerState(ControlMessenger controlMessenger)
        //{
        //    var borderColor = new ColorValue(controlMessenger);
        //    var addresseU = new GenericValue<TextureAddressMode>(controlMessenger);
        //    var addresseV = new GenericValue<TextureAddressMode>(controlMessenger);

        //    return new SamplerState(borderColor, addresseU, addresseV);
        //}

        //Transform2D CreateTransform2D(ControlMessenger controlMessenger)
        //{
        //    var uniformScale = new FloatValue(controlMessenger);
        //    var translate = new Vector2(0.0f, 0.0f);
        //    var scale = new Vector2(1.0f, 1.0f);
        //    var rotate = new FloatValue(controlMessenger);

        //    return new Transform2D(uniformScale, translate, scale, rotate);
        //}

        //TransformTexture CreateTransformTexture(ControlMessenger controlMessenger)
        //{
        //    var visible = new BooleanValue(controlMessenger);
        //    var samplerState = CreateSamplerState(controlMessenger);
        //    var transform2D = CreateTransform2D(controlMessenger);

        //    return new TransformTexture(visible, samplerState, transform2D);
        //}

        //DiffuseTexture CreateDiffuseTexture(ManagerMessenger managerMessenger, ControlMessenger controlMessenger)
        //{
        //    var prefabFactory = new PrefabFactory();
        //    var prefabManagerBase = new PrefabManagerBase(Guid.NewGuid(), prefabFactory, managerMessenger);
        //    var transformTexture = CreateTransformTexture(controlMessenger);
        //    var samplerState = CreateSamplerState(controlMessenger);

        //    return new DiffuseTexture(prefabManagerBase, transformTexture, samplerState);
        //}

        //MaskTexture CreateMaskTexture(ManagerMessenger managerMessenger, ControlMessenger controlMessenger)
        //{
        //    var prefabFactory = new PrefabFactory();
        //    var prefabManagerBase = new PrefabManagerBase(Guid.NewGuid(), prefabFactory, managerMessenger);
        //    var transformTexture = CreateTransformTexture(controlMessenger);
        //    var samplerState = CreateSamplerState(controlMessenger);
        //    var maskChannel = new GenericValue<MaskChannel>(controlMessenger);
        //    var invert = new BooleanValue(controlMessenger);
        //    var isEnabled = new BooleanValue(controlMessenger);

        //    return new MaskTexture(prefabManagerBase, transformTexture, samplerState, invert, maskChannel, isEnabled);
        //}

        //Material CreateMaterial(ManagerMessenger managerMessenger, ControlMessenger controlMessenger)
        //{
        //    var materialSettings = CreateMaterialSettings(controlMessenger);
        //    var diffuseTexture = CreateDiffuseTexture(managerMessenger, controlMessenger);
        //    var maskTexture = CreateMaskTexture(managerMessenger, controlMessenger);

        //    return new Material(materialSettings, diffuseTexture, maskTexture);
        //}

        //Mesh CreateMesh(ControlMessenger controlMessenger)
        //{
        //    var meshTypeSelector = new GenericValue<MeshType>(controlMessenger);
        //    var scale = new Vector3(1.0f, 1.0f, 1.0f);
        //    var offset = new Vector3();
        //    var radius = new FloatValue(controlMessenger);
        //    var height = new FloatValue(controlMessenger);
        //    var thickness = new FloatValue(controlMessenger);
        //    var tessellation = new IntegerValue(controlMessenger);

        //    var tessX = new IntegerValue(controlMessenger);
        //    var tessY = new IntegerValue(controlMessenger);
        //    var tessellationXY = new Integer2(tessX, tessY);
        //    var generateBackFace = new BooleanValue(controlMessenger);
        //    var visibility = new BooleanValue(controlMessenger);

        //    return new Mesh(meshTypeSelector, scale, offset, radius, height, thickness, tessellation, tessellationXY, generateBackFace, visibility);
        //}



        //var managerMessenger = new ManagerMessenger(Mapper);
        //var controlMessenger = new ControlMessenger(Mapper);

        //var material = CreateControl<Material>();// CreateMaterial(managerMessenger, controlMessenger);
        //var mesh = CreateControl<Mesh>();// CreateMesh(controlMessenger);

        //var modifierManager = new ModifierManager(new EntityModifierFactory(), new ManagerMessenger(Mapper));
        //var entity = new Entity(prefabService, mesh, material, modifierManager);


        public IPrefab CreatePrefab(PrefabService prefabService)
        {

            var entity = CreateControl<Entity>();

            var e = Mapper.Map(new EntityModel(), entity);
            LayerRepository.AddPrefab(e);

            return e;
        }

        public IPrefab CreatePrefab(PrefabService prefabService, IPrefabModel prefabModel)
        {
            var entity = Mapper.Map(prefabModel, CreatePrefab(prefabService));

            LayerRepository.AddPrefab(entity);

            return entity;
        }
    }
}
