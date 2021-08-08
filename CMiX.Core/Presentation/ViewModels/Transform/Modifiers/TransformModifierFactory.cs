// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Beat;

namespace CMiX.Core.Presentation.ViewModels
{
    public class TransformModifierFactory
    {
        public TransformModifierFactory()
        {

        }

        public MasterBeat MasterBeat { get; set; }

        public ITransformModifier CreateTransformModifier(TransformModifierNames transformModifierNames)
        {
            ITransformModifier transformModifier = null;

            switch (transformModifierNames)
            {
                case TransformModifierNames.Randomized:
                    transformModifier = CreateRandomized();
                    break;
            }

            return transformModifier;
        }

        public ITransformModifier CreateTransformModifier(ITransformModifierModel transformModifierModel)
        {
            ITransformModifier transformModifier = null;

            switch (transformModifierModel.Name)
            {
                case TransformModifierNames.Randomized:
                    transformModifier = CreateRandomized(transformModifierModel as RandomXYZModel);
                    break;
            }

            return transformModifier;
        }


        private RandomXYZ CreateRandomized()
        {
            var randomized = new RandomXYZ(new RandomXYZModel(), this.MasterBeat);
            return randomized;
        }

        private RandomXYZ CreateRandomized(RandomXYZModel randomXYZModel)
        {
            var randomized = new RandomXYZ(randomXYZModel, this.MasterBeat);
            return randomized;
        }
    }
}
