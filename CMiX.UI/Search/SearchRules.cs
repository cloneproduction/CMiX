// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using System;

namespace CMiX.UI.Search
{
    // The rule of every search field: the text contains the query, with no regard to case.
    public static class SearchRules
    {
        public static bool Matches(string? query, string? text)
        {
            if (string.IsNullOrEmpty(query))
                return true;

            return text != null && text.Contains(query, StringComparison.OrdinalIgnoreCase);
        }

        public static bool MatchesAny(string? query, string? first, string? second) =>
            Matches(query, first) || Matches(query, second);

        // The start of the query in the text, with no regard to case. An empty query marks no letter, so it gives -1.
        public static int IndexOfMatch(string? query, string? text)
        {
            if (string.IsNullOrEmpty(query) || text == null)
                return -1;

            return text.IndexOf(query, StringComparison.OrdinalIgnoreCase);
        }
    }
}
