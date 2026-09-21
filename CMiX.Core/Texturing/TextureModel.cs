// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMiX.Core.Texturing
{
    public record TextureModel : IControlModel
    {
        public Guid ID { get; init; } = Guid.NewGuid();
        public DiffuseTextureModel DiffuseTexture { get; set; } = new();
        public MaskTextureModel MaskTexture { get; set; } = new();
    }
}
