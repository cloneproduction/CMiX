// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;

namespace CMiX.Core.Presentation.ViewModels
{
    public interface IControl : IIDObject
    {
        IModel GetModel();
    }
}
