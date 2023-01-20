// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using FluentScheduler;

namespace CMiX.Core.Presentation.ViewModels.Scheduling
{
    public interface IUnit
    {
        //void SetUnit(TimeUnit timeUnit);
        Action<TimeUnit> SetScheduler { get; set; }
    }
}
