// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Rendering
{
    public record OutputMappingManagerModel : IControlModel
    {
        public Guid ID { get; init; } = Guid.NewGuid();
        public List<OutputMappingModel> Items { get; init; } = new();
    }
}
