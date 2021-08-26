// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Models;

namespace CMiX.Core.Presentation.ViewModels
{
    public class TransformModifierFactory
    {
        public TransformModifierFactory(Guid componentID)
        {
            ComponentID = componentID;
        }


        private Guid ComponentID { get; set; }


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
            var randomized = new RandomXYZ(new RandomXYZModel(), this.ComponentID);
            return randomized;
        }

        private RandomXYZ CreateRandomized(RandomXYZModel randomXYZModel)
        {
            var randomized = new RandomXYZ(randomXYZModel, this.ComponentID);
            return randomized;
        }
    }
}
