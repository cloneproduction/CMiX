// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CMiX.Core.BaseControls;

namespace CMiX.Core.Mapping
{
    public class ControlMappingAction : IMappingAction<GenericValue<int>, GenericValueModel<int>>
    {
        public ControlMappingAction()
        {
            
        }
        public void Process(GenericValue<int> source, GenericValueModel<int> destination, ResolutionContext context)
        {
            Console.WriteLine();
        }
    }
}
