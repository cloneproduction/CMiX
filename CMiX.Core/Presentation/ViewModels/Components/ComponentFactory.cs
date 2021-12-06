// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class ComponentFactory
    {
        public ComponentFactory()
        {
            components = new Dictionary<Type, Func<Component>>();
            componentsModel = new Dictionary<Type, Func<IComponentModel, Component>>();

            this.RegisterComponentType(typeof(Composition), () => new Composition(new CompositionModel(Guid.NewGuid())));
            this.RegisterComponentType(typeof(Layer), () => new Layer(new LayerModel(Guid.NewGuid())));
            this.RegisterComponentType(typeof(Scene), () => new Scene(new SceneModel(Guid.NewGuid())));
            this.RegisterComponentType(typeof(Entity), () => new Entity(new EntityModel(Guid.NewGuid())));
        }

        private readonly Dictionary<Type, Func<Component>> components;
        private readonly Dictionary<Type, Func<IComponentModel, Component>> componentsModel;

        public Component this[Type componentType, MasterBeat masterBeat] => CreateComponent(componentType, masterBeat);

        public Component CreateComponent(Type componentType, MasterBeat masterBeat = null)
        {
            if(componentType == null)
            {
                return components[typeof(Composition)]();
            }
            return components[componentType]();
        }

        public Component CreateComponent(IComponentModel componentModel)
        {
            return componentsModel[componentModel.GetType()](componentModel);
        }

        public Type[] RegisteredTypes => components.Keys.ToArray();

        public void RegisterComponentType(Type componentType, Func<Component> factoryMethod)
        {
            if (factoryMethod is null) return;

            components[componentType] = factoryMethod;
        }
    }
}
