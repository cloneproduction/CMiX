// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControl;
using CMiX.Core.Presentations.ViewModels.BaseControl;

namespace CMiX.Core.Presentations.Components
{
    public interface IComponentModel : IModel
    {
        StringValueModel Name { get; set; }
    }
}
