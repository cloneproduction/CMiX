// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using CMiX.Core.BaseControls;

namespace CMiX.Core.Compositing
{
    public interface IEntity : IControl, IDisposable
    {
        BooleanValue IsRenaming { get; set; }
        StringValue Name { get; set; }
        BooleanValue IsSelected { get; set; }
        BooleanValue Visibility { get; set; }
    }
}
