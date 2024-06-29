// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Execution;
using CMiX.Core.Compositing;
using CMiX.Core.Prefabs;

namespace CMiX.Core.Mapping
{
    public class CustomConverter : ITypeConverter<IPrefabModel, IPrefabModel>
    {
        public CustomConverter()
        {
            
        }
        public IPrefabModel Convert(IPrefabModel source, IPrefabModel destination, ResolutionContext context)
        {
            if (source is Layer)
                return context.Mapper.Map(source, destination);
            else return null;
        }
    }
}
