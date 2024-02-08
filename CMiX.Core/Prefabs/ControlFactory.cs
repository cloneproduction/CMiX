// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Services;

namespace CMiX.Core.Prefabs
{
    public class ControlFactory
    {
        public ControlFactory(Dictionary<Type, Func<IControl>> factories, Dictionary<Type, Func<IControlModel, IControl>> modelToControlFactories, PrefabRepositories prefabRepositories)
        {
            Factories = factories;
            ModelToControlFactories = modelToControlFactories;
            PrefabRepositories = prefabRepositories;
        }

        internal PrefabRepositories PrefabRepositories { get; set; }
        internal Dictionary<Type, Func<IControlModel, IControl>> ModelToControlFactories { get; set; }
        internal Dictionary<Type, Func<IControl>> Factories { get; set; }

        public IControl Create(Type type) 
        {
            if (!Factories.TryGetValue(type, out var factory) || factory is null)
                throw new ArgumentOutOfRangeException(nameof(type), $"type '{type}' is not registered");

            var f = factory();
            PrefabRepositories.AddControl(f);
            return f;
        }

        public IControl Create(IControlModel controlModel)
        {
            var type = controlModel.GetType();

            if (!ModelToControlFactories.TryGetValue(type, out var factory) || factory is null)
                throw new ArgumentOutOfRangeException(nameof(type), $"type '{type}' is not registered");

            var f = factory(controlModel);
            PrefabRepositories.AddControl(f);
            return f;
        }

        public IPrefab GetPrefab(Guid id)
        {
            //if (Factories == null)
            //    return null;

            //var factory = Factories.FirstOrDefault(x => x.GetPrefab(id) != null);

            //if (factory == null)
            //    return null;

            //var prefab = factory.GetPrefab(id);
            //return prefab;
            return null;
        }
    }
}
