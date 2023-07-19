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
            typePairs.Add(typeof(RandomHSV), typeof(RandomHSVModel));
            typePairs.Add(typeof(RandomXYZ), typeof(RandomXYZModel));
            typePairs.Add(typeof(RandomPosition), typeof(RandomPositionModel));
            typePairs.Add(typeof(LinearXYZ), typeof(LinearXYZModel));
            typePairs.Add(typeof(LFO), typeof(LFOModel));
            typePairs.Add(typeof(RandomScale), typeof(RandomScaleModel));
            typePairs.Add(typeof(TransformSRT), typeof(TransformSRTModel));
            typePairs.Add(typeof(Translate), typeof(TranslateModel));
            typePairs.Add(typeof(Scale), typeof(ScaleModel));
            typePairs.Add(typeof(Rotation), typeof(RotationModel));
            typePairs.Add(typeof(Stepper), typeof(StepperModel));
        }

        private Dictionary<Type, Type> typePairs = new Dictionary<Type, Type>();

        public IModifier Create(Type modifierType)
        {
            return (IModifier)Activator.CreateInstance(modifierType);
        }

        public IModifier Create(IModifierModel modifierModel)
        {
            Type viewModelType = typePairs.FirstOrDefault(x => x.Value == modifierModel.GetType()).Key;
            return (IModifier)ControlMessenger.Mapper.Map(modifierModel, modifierModel.GetType(), viewModelType);
        }

        public IModifierModel CreateModel(IModifier modifier)
        {
            Type modelType = typePairs.GetValueOrDefault(modifier.GetType());
            return (IModifierModel)ControlMessenger.Mapper.Map(modifier, modifier.GetType(), modelType);
        }
    }
}
