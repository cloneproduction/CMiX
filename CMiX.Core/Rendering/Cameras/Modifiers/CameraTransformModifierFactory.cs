// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modifiers;

namespace CMiX.Core.Rendering.Cameras.Modifiers
{
    public class CameraTransformModifierFactory : IModifierFactory
    {
        public CameraTransformModifierFactory()
        {

        }

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
            return new CameraLFO(new CameraLFOModel());
        }

        private CameraLFO CreateCameraLFO(CameraLFOModel cameraLFOModel)
        {
            return new CameraLFO(cameraLFOModel);
        }

        private CameraRandom CreateCameraRandom()
        {
            return new CameraRandom(new CameraRandomModel());
        }

        private CameraRandom CreateCameraRandom(CameraRandomModel cameraRandomModel)
        {
            return new CameraRandom(cameraRandomModel);
        }

        public IModifierModel CreateModel(IModifier modifier)
        {
            return null;
        }
    }
}
