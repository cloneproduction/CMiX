using CMiX.Core.Networking;
using Xunit;

namespace CMiX.Core.Tests
{
    public class StreamPositionTests
    {
        [Fact]
        public void Parse_ValidId_ReturnsMillisecondsAndSequence()
        {
            var position = StreamPosition.Parse("1693737600000-3");

            Assert.Equal(1693737600000, position.Milliseconds);
            Assert.Equal(3, position.Sequence);
        }

        [Fact]
        public void ToStringThenParse_RoundTrips()
        {
            var position = new StreamPosition(1693737600000, 3);

            var roundTripped = StreamPosition.Parse(position.ToString());

            Assert.Equal(position, roundTripped);
        }

        [Fact]
        public void Parse_NonNumeric_ThrowsFormatException()
        {
            var exception = Assert.Throws<FormatException>(() => StreamPosition.Parse("abc"));
            Assert.Contains("abc", exception.Message);
        }

        [Fact]
        public void Parse_TooManyParts_ThrowsFormatException()
        {
            Assert.Throws<FormatException>(() => StreamPosition.Parse("1-2-3"));
        }

        [Fact]
        public void TryParse_Null_ReturnsFalse()
        {
            Assert.False(StreamPosition.TryParse(null, out _));
        }

        [Fact]
        public void TryParse_InvalidValue_ReturnsFalse()
        {
            Assert.False(StreamPosition.TryParse("x", out _));
        }

        [Fact]
        public void CompareTo_DifferentMilliseconds_OrdersByMilliseconds()
        {
            var earlier = new StreamPosition(100, 5);
            var later = new StreamPosition(200, 0);

            Assert.True(earlier < later);
            Assert.True(later > earlier);
        }

        [Fact]
        public void CompareTo_EqualMilliseconds_OrdersBySequence()
        {
            var lower = new StreamPosition(100, 1);
            var higher = new StreamPosition(100, 2);

            Assert.True(lower < higher);
            Assert.True(higher > lower);
        }

        [Fact]
        public void Zero_IsLessThanFirstEntry()
        {
            Assert.True(StreamPosition.Zero < new StreamPosition(1, 0));
        }

        [Fact]
        public void Equality_OfTwoParsedEqualValues_IsTrue()
        {
            var first = StreamPosition.Parse("100-1");
            var second = StreamPosition.Parse("100-1");

            Assert.Equal(first, second);
            Assert.True(first == second);
        }
    }
}
