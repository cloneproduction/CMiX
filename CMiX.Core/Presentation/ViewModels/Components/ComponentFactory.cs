// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using CMiX.Core.Models;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class ComponentFactory
    {
        public ComponentFactory()
        {
            components = new Dictionary<Type, Func<Component>>();
        }

        private readonly Dictionary<Type, Func<Component>> components;

        public Component this[Type componentType] => CreateComponent(componentType);

        public Component CreateComponent(Type componentType)
        {
            return components[componentType]();
        }

        public Component CreateComponent(IComponentModel componentModel)
        {
            Component component = components[componentModel.GetType()]();
            component.SetViewModel(componentModel);
            return component;
        }

        public Type[] RegisteredTypes => components.Keys.ToArray();

        public void RegisterComponentType(Type componentType, Func<Component> factoryMethod)
        {
            if (factoryMethod is null) return;

            components[componentType] = factoryMethod;
        }
    }
}
