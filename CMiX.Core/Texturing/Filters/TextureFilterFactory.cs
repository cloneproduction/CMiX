// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Modifiers;
using CMiX.Core.Networking;
using CMiX.Core.Transformation.Modifiers;

namespace CMiX.Core.Texturing.Filters
{
    public class TextureFilterFactory : IModifierFactory
    {
        public TextureFilterFactory()
        {
            typePairs.Add(typeof(HSCB), typeof(HSCBModel));
            typePairs.Add(typeof(Invert), typeof(InvertModel));
            typePairs.Add(typeof(Blur), typeof(BlurModel));
            typePairs.Add(typeof(Edge), typeof(EdgeModel));
            typePairs.Add(typeof(TransformTexture), typeof(TransformTextureModel));
            typePairs.Add(typeof(Pixelate), typeof(PixelateModel));
            typePairs.Add(typeof(Echo), typeof(EchoModel));
            typePairs.Add(typeof(Feedback), typeof(FeedbackModel));
            typePairs.Add(typeof(TriColor), typeof(TriColorModel));
            typePairs.Add(typeof(RandomUV), typeof(RandomUVModel));
            typePairs.Add(typeof(LFOUV), typeof(LFOUVModel));
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
