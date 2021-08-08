// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMiX.Core.Presentation.ViewModels.Components
{
    public class ComponentFactory
    {
        public ComponentFactory()
        {
            components = new Dictionary<ComponentType, Func<Component>>();
        }

        private readonly Dictionary<ComponentType, Func<Component>> components;

        public Component this [ComponentType componentType] => CreateComponent(componentType);

        public Component CreateComponent(ComponentType componentType) => components[componentType]();

        public ComponentType[] RegisteredTypes => components.Keys.ToArray();

        public void RegisterComponentType(ComponentType componentType, Func<Component> factoryMethod)
        {
            //if (string.IsNullOrEmpty(componentType)) return;
            if (factoryMethod is null) return;

            components[componentType] = factoryMethod;
        }
    }
}
