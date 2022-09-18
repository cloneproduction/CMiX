// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Presentation.ViewModels.Service;

namespace CMiX.Core.Presentation.ViewModels
{
    public class CameraTransformModifierFactory : IModifierFactory
    {
        public CameraTransformModifierFactory(CompositionService compositionService)
        {
            CompositionService = compositionService;
        }

        private CompositionService CompositionService { get; set; }

        public IModifier Create(Type modifierType)
        {
            if (modifierType == typeof(CameraLFO))
                return CreateCameraLFO();

            return null;
        }

        public IModifier Create(IModifierModel modifierModel)
        {
            if (modifierModel is CameraLFOModel cameraLFOModel)
                return CreateCameraLFO(cameraLFOModel);

            return null;
        }

        private CameraLFO CreateCameraLFO()
        {
            return new CameraLFO(new CameraLFOModel(), CompositionService);
        }

        private CameraLFO CreateCameraLFO(CameraLFOModel cameraLFOModel)
        {
            return new CameraLFO(cameraLFOModel, CompositionService);
        }
    }
}
