// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using CMiX.Core.Presentation.ViewModels.Beat;
using CMiX.Core.Presentation.ViewModels.Modifiers;
using CMiX.Core.Models;
using CMiX.Core.Presentation.ViewModels.Service;

namespace CMiX.Core.Presentation.ViewModels
{
    public class ColorModifierFactory : IModifierFactory
    {
        public ColorModifierFactory(CompositionService compositionService)
        {
            CompositionService = compositionService;
         }

        public CompositionService CompositionService { get; set; }

        public IModifier Create(Type modifierType)
        {
            if (modifierType == typeof(RandomHSV))
                return CreateRandomHSV();

            return null;
        }

        public IModifier Create(IModifierModel modifierModel)
        {
            if (modifierModel is RandomHSVModel randomHSVModel)
                return CreateRandomHSV(randomHSVModel);

            return null;
        }


        private RandomHSV CreateRandomHSV()
        {
            return new RandomHSV(new RandomHSVModel(), CompositionService);
        }

        private RandomHSV CreateRandomHSV(RandomHSVModel randomHSVModel)
        {
            return new RandomHSV(randomHSVModel, CompositionService);
        }
    }
}
