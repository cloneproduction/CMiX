using Xunit;

namespace CMiX.Core.Tests
{
    public class DerivedIdTests
    {
        [Fact]
        public void Of_SameOwnerAndName_GivesTheSameID()
        {
            var owner = Guid.NewGuid();

            Assert.Equal(DerivedId.Of(owner, "Name"), DerivedId.Of(owner, "Name"));
        }

        [Fact]
        public void Of_OtherOwnerOrOtherName_GivesAnotherID()
        {
            var owner = Guid.NewGuid();

            Assert.NotEqual(DerivedId.Of(owner, "Name"), DerivedId.Of(Guid.NewGuid(), "Name"));
            Assert.NotEqual(DerivedId.Of(owner, "Name"), DerivedId.Of(owner, "IsSelected"));
        }
    }
}
