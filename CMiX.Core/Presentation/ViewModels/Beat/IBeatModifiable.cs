// Copyright (c) CloneProduction Shanghai Company Limited (https://cloneproduction.net/)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

namespace CMiX.Core.Presentation.ViewModels.Beat
{
    public interface IBeatModifiable : IBeatable
    {
        BeatModifier BeatModifier { get; set; }
    }
}
