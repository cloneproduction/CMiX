// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Presentations.ViewModels.Modifiers;
using CMiX.Core.Presentations.Service;
using CMiX.Core.Presentations.Modifiers;
using CMiX.Core.Presentations.Modifiers.Camera;

namespace CMiX.Core.Presentations.ViewModels
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

            if (modifierType == typeof(CameraRandom))
                return CreateCameraRandom();

            return null;
        }

        public IModifier Create(IModifierModel modifierModel)
        {
            if (modifierModel is CameraLFOModel cameraLFOModel)
                return CreateCameraLFO(cameraLFOModel);

            if (modifierModel is CameraRandomModel cameraRandomModel)
                return CreateCameraRandom(cameraRandomModel);

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

        private CameraRandom CreateCameraRandom()
        {
            return new CameraRandom(new CameraRandomModel(), CompositionService);
        }

        private CameraRandom CreateCameraRandom(CameraRandomModel cameraRandomModel)
        {
            return new CameraRandom(cameraRandomModel, CompositionService);
        }

        public IModifierModel CreateModel(IModifier modifier)
        {
            return null;
        }
    }
}
