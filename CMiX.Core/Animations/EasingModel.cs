// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;
using CMiX.Core.ViewModels;

namespace CMiX.Core.Animations
{
    public class EasingModel : IControlModel
    {
        public EasingModel()
        {
            ID = Guid.NewGuid();
            IsEnabled = new BooleanValueModel(false);
            Function = new GenericValueModel<EasingFunction>(EasingFunction.Linear);
            Mode = new GenericValueModel<EasingMode>(EasingMode.In);
        }

        public BooleanValueModel IsEnabled { get; set; }
        public Guid ID { get; set; }
        public GenericValueModel<EasingFunction> Function { get; set; }
        public GenericValueModel<EasingMode> Mode { get; set; }
    }
}
