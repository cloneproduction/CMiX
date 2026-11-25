// SPDX-FileCopyrightText: 2017-2026 CloneProduction Shanghai Company Limited and CMiX contributors
// SPDX-License-Identifier: LGPL-3.0-or-later

using CMiX.UI.Search;
using Xunit;

namespace CMiX.UI.Tests
{
    public class SearchRulesTests
    {
        [Fact]
        public void Matches_AnEmptyQuery_MatchesEveryText()
        {
            Assert.True(SearchRules.Matches("", "Blur"));
            Assert.True(SearchRules.Matches("", ""));
            Assert.True(SearchRules.Matches(null, "Blur"));
        }

        [Fact]
        public void Matches_ADifferentCase_Matches()
        {
            Assert.True(SearchRules.Matches("BLUR", "Blur"));
            Assert.True(SearchRules.Matches("blur", "BLUR"));
        }

        [Fact]
        public void Matches_ANullText_MatchesOnlyTheEmptyQuery()
        {
            Assert.True(SearchRules.Matches("", null));
            Assert.True(SearchRules.Matches(null, null));
            Assert.False(SearchRules.Matches("blur", null));
        }

        [Fact]
        public void Matches_APartOfAWord_Matches()
        {
            Assert.True(SearchRules.Matches("lur", "Blur"));
            Assert.False(SearchRules.Matches("blurs", "Blur"));
        }

        [Fact]
        public void MatchesAny_TheSecondText_Matches()
        {
            Assert.True(SearchRules.MatchesAny("effect", "Blur", "Effect"));
            Assert.True(SearchRules.MatchesAny("effect", null, "Effect"));
        }

        [Fact]
        public void IndexOfMatch_APartOfAWord_GivesItsStart()
        {
            Assert.Equal(1, SearchRules.IndexOfMatch("lur", "Blur"));
            Assert.Equal(0, SearchRules.IndexOfMatch("BL", "Blur"));
        }

        [Fact]
        public void IndexOfMatch_NoMatch_GivesMinusOne()
        {
            Assert.Equal(-1, SearchRules.IndexOfMatch("glow", "Blur"));
            Assert.Equal(-1, SearchRules.IndexOfMatch("blur", null));
        }

        [Fact]
        public void IndexOfMatch_AnEmptyQuery_GivesMinusOne()
        {
            Assert.Equal(-1, SearchRules.IndexOfMatch("", "Blur"));
            Assert.Equal(-1, SearchRules.IndexOfMatch(null, "Blur"));
        }

        [Fact]
        public void MatchesAny_NeitherText_DoesNotMatch()
        {
            Assert.False(SearchRules.MatchesAny("glow", "Blur", "Effect"));
            Assert.False(SearchRules.MatchesAny("glow", null, null));
        }
    }
}
