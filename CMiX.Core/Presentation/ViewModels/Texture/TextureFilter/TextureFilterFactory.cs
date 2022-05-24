// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Modifiers;

namespace CMiX.Core.Presentation.ViewModels
{
    public class TextureFilterFactory : IModifierFactory
    {
        public TextureFilterFactory()
        {

        }

        public IModifier Create(Type modifierType)
        {
            if(modifierType == typeof(HSCB))
                return new HSCB(new HSCBModel());

            if(modifierType == typeof(Invert))
                return new Invert(new InvertModel());

            if(modifierType == typeof(Blur))
                return new Blur(new BlurModel());

            if(modifierType == typeof(Edge))
                return new Edge(new EdgeModel());

            if(modifierType == typeof(TransformTexture))
                return new TransformTexture(new TransformTextureModel());

            if (modifierType == typeof(Pixelate))
                return new Pixelate(new PixelateModel());

            if (modifierType == typeof(Echo))
                return new Echo(new EchoModel());

            if (modifierType == typeof(Feedback))
                return new Feedback(new FeedbackModel());

            if (modifierType == typeof(TriColor))
                return new TriColor(new TriColorModel());

            return null;
        }

        public IModifier Create(IModifierModel modifierModel)
        {
            if (modifierModel is HSCBModel hSCBModel)
                return new HSCB(hSCBModel);

            if(modifierModel is InvertModel invertModel)
                return new Invert(invertModel);

            if (modifierModel is BlurModel blurModel)
                return new Blur(blurModel);

            if (modifierModel is EdgeModel edgeModel)
                return new Edge(edgeModel);

            if (modifierModel is TransformTextureModel modelTexture)
                return new TransformTexture(modelTexture);

            if (modifierModel is PixelateModel pixelateModel)
                return new Pixelate(pixelateModel);

            if (modifierModel is EchoModel echoModel)
                return new Echo(echoModel);

            if (modifierModel is FeedbackModel feedbackModel)
                return new Feedback(feedbackModel);

            if (modifierModel is TriColorModel triColorModel)
                return new TriColor(triColorModel);

            return null;
        }

        public void SetMasterBeat(MasterBeat masterBeat)
        {
            throw new NotImplementedException();
        }
    }
}
