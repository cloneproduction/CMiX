// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Colors.Modifiers;
using CMiX.Core.Modifiers;
using CMiX.Core.Networking;

namespace CMiX.Core.Transformation.Modifiers
{
    public class ModifierFactory : IModifierFactory
    {
        public ModifierFactory()
        {

        }

        public IModifier Create(Type modifierType)
        {
            if (modifierType == typeof(RandomHSV))
                return ControlMessenger.Mapper.Map<RandomHSV>(new RandomHSVModel());

            if (modifierType == typeof(RandomXYZ))
                return ControlMessenger.Mapper.Map<RandomXYZ>(new RandomXYZModel());

            if (modifierType == typeof(RandomPosition))
                return ControlMessenger.Mapper.Map<RandomPosition>(new RandomPositionModel());

            if (modifierType == typeof(LinearXYZ))
                return ControlMessenger.Mapper.Map<LinearXYZ>(new LinearXYZModel());

            if (modifierType == typeof(LFO))
                return ControlMessenger.Mapper.Map<LFO>(new LFOModel());

            if (modifierType == typeof(RandomScale))
                return ControlMessenger.Mapper.Map<RandomScale>(new RandomScaleModel());

            if (modifierType == typeof(TransformSRT))
                return ControlMessenger.Mapper.Map<TransformSRT>(new TransformSRTModel());

            if (modifierType == typeof(Translate))
                return ControlMessenger.Mapper.Map<Translate>(new TranslateModel());

            if (modifierType == typeof(Scale))
                return ControlMessenger.Mapper.Map<Scale>(new ScaleModel());

            if (modifierType == typeof(Rotation))
                return ControlMessenger.Mapper.Map<Rotation>(new RotationModel());

            if (modifierType == typeof(Stepper))
                return ControlMessenger.Mapper.Map<Stepper>(new StepperModel());

            return null;
        }

        public IModifier Create(IModifierModel modifierModel)
        {
            if (modifierModel is RandomHSVModel randomHSVModel)
                return ControlMessenger.Mapper.Map<RandomHSV>(randomHSVModel);

            if (modifierModel is RandomXYZModel randomXYZModel)
                return ControlMessenger.Mapper.Map<RandomXYZ>(randomXYZModel);

            if (modifierModel is RandomPositionModel randomPositionModel)
                return ControlMessenger.Mapper.Map<RandomPosition>(randomPositionModel);

            if (modifierModel is LinearXYZModel linearXYZModel)
                return ControlMessenger.Mapper.Map<LinearXYZ>(linearXYZModel);

            if (modifierModel is LFOModel lfoModel)
                return ControlMessenger.Mapper.Map<LFO>(lfoModel);

            if (modifierModel is RandomScaleModel randomScaleModel)
                return ControlMessenger.Mapper.Map<RandomScale>(randomScaleModel);

            if (modifierModel is TransformSRTModel transformSRTModel)
                return ControlMessenger.Mapper.Map<TransformSRT>(transformSRTModel);

            if (modifierModel is TranslateModel translateModel)
                return ControlMessenger.Mapper.Map<Translate>(translateModel);

            if (modifierModel is ScaleModel scaleModel)
                return ControlMessenger.Mapper.Map<Scale>(scaleModel);

            if (modifierModel is RotationModel rotationModel)
                return ControlMessenger.Mapper.Map<Rotation>(rotationModel);

            if (modifierModel is StepperModel stepperModel)
                return ControlMessenger.Mapper.Map<Stepper>(stepperModel);

            return null;
        }

        public IModifierModel CreateModel(IModifier modifier)
        {
            var type = modifier.GetType();

            if (type == typeof(RandomHSV))
                return ControlMessenger.Mapper.Map<RandomHSVModel>(modifier);

            if (type == typeof(RandomXYZ))
                return ControlMessenger.Mapper.Map<RandomXYZModel>(modifier);

            if (type == typeof(RandomPosition))
                return ControlMessenger.Mapper.Map<RandomPositionModel>(modifier);

            if (type == typeof(LinearXYZ))
                return ControlMessenger.Mapper.Map<LinearXYZModel>(modifier);

            if (type == typeof(LFO))
                return ControlMessenger.Mapper.Map<LFOModel>(modifier);

            if (type == typeof(RandomScale))
                return ControlMessenger.Mapper.Map<RandomScaleModel>(modifier);

            if (type == typeof(TransformSRT))
                return ControlMessenger.Mapper.Map<TransformSRTModel>(modifier);

            if (type == typeof(Translate))
                return ControlMessenger.Mapper.Map<TranslateModel>(modifier);

            if (type == typeof(Scale))
                return ControlMessenger.Mapper.Map<ScaleModel>(modifier);

            if (type == typeof(Rotation))
                return ControlMessenger.Mapper.Map<RotationModel>(modifier);

            if (type == typeof(Stepper))
                return ControlMessenger.Mapper.Map<StepperModel>(modifier);

            return null;
        }
    }
}
