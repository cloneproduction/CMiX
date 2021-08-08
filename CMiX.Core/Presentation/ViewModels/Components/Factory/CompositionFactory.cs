// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;

namespace CMiX.Core.Presentation.ViewModels.Components.Factories
{
    public class CompositionFactory : IComponentFactory
    {
        public CompositionFactory(IProject parentProject)
        {
            ParentProject = parentProject as Project;
        }

        private Project ParentProject { get; set; }

        public Component CreateComponent()
        {
            var model = new CompositionModel(Guid.NewGuid());
            var component = new Composition(model);
            return component;
        }

        public Component CreateComponent(IComponentModel model)
        {
            var component = new Composition(model as CompositionModel);
            return component;
        }
    }
}
