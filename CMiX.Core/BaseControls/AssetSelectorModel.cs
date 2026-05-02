// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.BaseControls
{
    public record AssetSelectorModel : IControlModel
    {
        public Guid ID { get; init; } = Guid.NewGuid();
        public GenericValueModel<string> FilePath { get; init; } = new(string.Empty);
        public string AssetType { get; init; } = string.Empty;
    }
}
