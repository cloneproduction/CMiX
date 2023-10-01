// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Colors.Modifiers;
using CMiX.Core.Modifiers;

namespace CMiX.Core.Transformation.Modifiers
{
    public class EntityModifierFactory : ModifierFactory
    {
        public EntityModifierFactory()
        {
            TypePairs.Add(typeof(RandomHSV), typeof(RandomHSVModel));
            TypePairs.Add(typeof(RandomXYZ), typeof(RandomXYZModel));
            TypePairs.Add(typeof(RandomPosition), typeof(RandomPositionModel));
            TypePairs.Add(typeof(LinearXYZ), typeof(LinearXYZModel));
            TypePairs.Add(typeof(LFO), typeof(LFOModel));
            TypePairs.Add(typeof(RandomScale), typeof(RandomScaleModel));
            TypePairs.Add(typeof(TransformSRT), typeof(TransformSRTModel));
            TypePairs.Add(typeof(Translate), typeof(TranslateModel));
            TypePairs.Add(typeof(Scale), typeof(ScaleModel));
            TypePairs.Add(typeof(Rotation), typeof(RotationModel));
            TypePairs.Add(typeof(Stepper), typeof(StepperModel));
        }
    }
}
