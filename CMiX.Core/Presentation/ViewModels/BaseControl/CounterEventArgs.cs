// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMiX.Core.Presentation.ViewModels
{
    public class CounterEventArgs
    {
        public CounterEventArgs(int value)
        {
            Value = value;
        }

        public int Value { get; set; }
    }
}
