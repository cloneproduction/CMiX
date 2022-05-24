// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.Models;
using System;

namespace CMiX.Core.Models
{
    public interface IRangeModel : IModel
    {
        double Minimum { get; set; }
        double Maximum { get; set; }
    }
}
