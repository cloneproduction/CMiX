// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Prefabs
{
    public class ControlFactory
    {
        public ControlFactory(Dictionary<Type, Func<IControl>> factories, Dictionary<Type, Func<IControlModel, IControl>> modelToControlFactories, ControlRepository controlRepository)
        {
            Factories = factories;
            ModelToControlFactories = modelToControlFactories;
            ControlRepository = controlRepository;
        }

        internal ControlRepository ControlRepository { get; set; }
        internal Dictionary<Type, Func<IControlModel, IControl>> ModelToControlFactories { get; set; }
        internal Dictionary<Type, Func<IControl>> Factories { get; set; }

        public IControl Create(Type type) 
        {
            if (!Factories.TryGetValue(type, out var factory) || factory is null)
                throw new ArgumentOutOfRangeException(nameof(type), $"type '{type}' is not registered");

            var f = factory();
            ControlRepository.AddControl(f);
            return f;
        }

        public IControl Create(IControlModel controlModel)
        {
            var type = controlModel.GetType();

            if (!ModelToControlFactories.TryGetValue(type, out var factory) || factory is null)
                throw new ArgumentOutOfRangeException(nameof(type), $"type '{type}' is not registered");

            var f = factory(controlModel);
            ControlRepository.AddControl(f);
            return f;
        }

        public IControl GetPrefab(Guid id)
        {
            return ControlRepository.GetPrefab(id);
        }
    }
}
