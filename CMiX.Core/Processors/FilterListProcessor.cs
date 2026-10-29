// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.Core.Texturing;
using CMiX.Core.Texturing.Filters;

namespace CMiX.Core.Processors
{
    // Gives the ids of the texture filters that run, in the order of the list.
    // A hidden filter and a filter whose blend is off are not in the result.
    // The list is built again at each call, because a list of filters is short.
    public class FilterListProcessor
    {
        private readonly VisibilityProcessor visibility;

        public FilterListProcessor(VisibilityProcessor visibility)
        {
            this.visibility = visibility;
        }

        public IReadOnlyList<Guid> GetEnabledIds(ITextureModifiable owner) =>
            owner == null
                ? Array.Empty<Guid>()
                : owner.TextureModifierManager.ManagerData.Items
                    .Where(item => !visibility.IsHidden(item.ID) && IsEnabled(item))
                    .Select(item => item.ID)
                    .ToArray();

        private static bool IsEnabled(IControl item) => item is ITextureFilter filter && filter.Blend.IsEnabled.Value;
    }
}
