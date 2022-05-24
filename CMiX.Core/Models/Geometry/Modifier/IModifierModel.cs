// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Models
{
    public interface IModifierModel : IModel
    {
        ToggleButtonModel Visible { get; set; }
    }
}
