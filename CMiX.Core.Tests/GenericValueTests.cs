using CMiX.Core.BaseControls;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Core.Tests
{
    // A GenericValue: its defaults, a momentary value, and a write that sends nothing.
    public class GenericValueTests
    {
        [Fact]
        public void SetDefaults_OfAValue_CopiesItToOriginalValue()
        {
            var provider = TestServiceProviderFactory.Create();
            var value = provider.GetRequiredService<GenericValue<int>>();

            value.Value = 5;
            value.SetDefaults();

            Assert.Equal(5, value.Value);
            Assert.Equal(5, value.OriginalValue);
        }

        [Fact]
        public void Reset_AfterFromModel_GivesTheDefaultNotTheLoadedValue()
        {
            var provider = TestServiceProviderFactory.Create();
            var value = provider.GetRequiredService<GenericValue<int>>();

            value.Value = 5;
            value.SetDefaults();
            value.FromModel(new GenericValueModel<int>(9));
            value.Reset();

            Assert.Equal(5, value.Value);
        }

        private static GenericValue<bool> CreateActiveBool(FakeValueSync sync)
        {
            var control = new GenericValue<bool>(sync, new ControlActivationService());
            control.Activate();
            return control;
        }

        [Fact]
        public void Value_OfAMomentaryValue_WritesNoUndoStep()
        {
            var sync = new FakeValueSync();
            var control = CreateActiveBool(sync);
            control.IsMomentary = true;

            control.Value = true;
            control.Value = false;

            Assert.Equal(0, sync.Records);
            Assert.Equal(0, sync.Commits);
        }

        [Fact]
        public void Value_OfAMomentaryValue_StillSendsTheWireMessage()
        {
            var sync = new FakeValueSync();
            var control = CreateActiveBool(sync);
            control.IsMomentary = true;

            control.Value = true;
            control.Value = false;

            Assert.Equal(2, sync.Sends);
        }

        [Fact]
        public void Value_OfANormalValue_StillWritesAnUndoStep()
        {
            var sync = new FakeValueSync();
            var control = CreateActiveBool(sync);

            control.Value = true;

            Assert.Equal(1, sync.Commits);
        }

        private static GenericValue<double> CreateActiveDouble(FakeValueSync sync)
        {
            var control = new GenericValue<double>(sync, new ControlActivationService());
            control.Activate();
            return control;
        }

        [Fact]
        public void SuppressSending_Write_SendsNothingAndLeavesNoUndoStep()
        {
            var sync = new FakeValueSync();
            var control = CreateActiveDouble(sync);

            using (control.SuppressSending())
                control.Value = 5;

            Assert.Equal(5d, control.Value);
            Assert.Empty(sync.Calls);
        }

        [Fact]
        public void SuppressSending_WriteAfterScope_SendsAgain()
        {
            var sync = new FakeValueSync();
            var control = CreateActiveDouble(sync);

            using (control.SuppressSending())
                control.Value = 5;
            control.Value = 6;

            Assert.Equal(new[] { "Commit" }, sync.Calls);
        }

        [Fact]
        public void SuppressSending_WriteAfterInnerScopeEnds_StaysSuppressedUntilTheOuterScopeEnds()
        {
            var sync = new FakeValueSync();
            var control = CreateActiveDouble(sync);

            using (control.SuppressSending())
            {
                using (control.SuppressSending())
                    control.Value = 1;
                control.Value = 2;
            }

            Assert.False(control.IsSendingSuppressed);
            Assert.Empty(sync.Calls);
        }

        [Fact]
        public void SuppressSending_OnAControlThatIsNotActive_DoesNotActivateIt()
        {
            var provider = TestServiceProviderFactory.Create();
            var control = provider.GetRequiredService<GenericValue<double>>();

            using (control.SuppressSending())
                control.Value = 3;

            Assert.False(control.IsActive);
        }
    }
}
