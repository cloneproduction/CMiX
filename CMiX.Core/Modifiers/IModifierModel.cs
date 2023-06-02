// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Modifiers
{
    public interface IModifierModel : IControlModel
    {
        BooleanValueModel Visible { get; set; }
    }
}
