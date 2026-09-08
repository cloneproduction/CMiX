using System;
using System.ComponentModel;
using System.Linq;
using Avalonia.Headless.XUnit;
using CMiX.Core.Animations;
using CMiX.Core.Modulation.Modulators;
using CMiX.Studio.Avalonia.Animations;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CMiX.Studio.Avalonia.Tests
{
    // The beat clock used to tick every 16 ms for the whole process lifetime and to advance all
    // fifteen animated doubles whether or not anything read them. These tests pin the replacement
    // rule: the clock only runs while something observes a position.
    public class BeatAnimationsTests
    {
        [AvaloniaFact]
        public void Clock_StaysIdle_WhileNothingObservesAPosition()
        {
            var animations = new BeatAnimations();

            animations.MakeStoryBoard(new[] { 500f, 1000f, 2000f });

            Assert.False(animations.IsRunning);
            foreach (var animatedDouble in animations.AnimatedDoubles)
                Assert.False(animatedDouble.IsObserved);
        }

        [AvaloniaFact]
        public void Clock_Starts_WhenABindingObservesAPosition()
        {
            var animations = new BeatAnimations();
            animations.MakeStoryBoard(new[] { 500f, 1000f, 2000f });

            PropertyChangedEventHandler handler = (s, e) => { };
            animations.AnimatedDoubles[1].PropertyChanged += handler;

            Assert.True(animations.IsRunning);
            Assert.True(animations.AnimatedDoubles[1].IsObserved);
            Assert.False(animations.AnimatedDoubles[0].IsObserved);

            animations.AnimatedDoubles[1].PropertyChanged -= handler;

            Assert.False(animations.AnimatedDoubles[1].IsObserved);
        }

        [AvaloniaFact]
        public void Clock_Starts_WhenABeatRandomModulatorListensForPulses()
        {
            var animations = new BeatAnimations();
            animations.MakeStoryBoard(new[] { 500f, 1000f, 2000f });

            EventHandler handler = (s, e) => { };
            animations.AnimatedDoubles[0].PositionChanged += handler;

            Assert.True(animations.IsRunning);
            Assert.True(animations.AnimatedDoubles[0].IsObserved);
        }

        // Multiply and divide only move the index into the period table, so the table they announce
        // is the one already built. Rebuilding it anyway replaced all fifteen instances, which took
        // every observer with it and restarted the stopwatch the positions are a closed form of.
        [AvaloniaFact]
        public void MakeStoryBoard_OnAnUnchangedPeriodTable_KeepsTheInstancesAndTheirObservers()
        {
            var animations = new BeatAnimations();
            animations.MakeStoryBoard(new[] { 500f, 1000f, 2000f });

            var built = animations.AnimatedDoubles.ToList();
            EventHandler handler = (s, e) => { };
            animations.AnimatedDoubles[1].PositionChanged += handler;

            animations.MakeStoryBoard(new[] { 500f, 1000f, 2000f });

            Assert.Equal(built.Count, animations.AnimatedDoubles.Count);
            for (var i = 0; i < built.Count; i++)
                Assert.Same(built[i], animations.AnimatedDoubles[i]);

            Assert.True(animations.AnimatedDoubles[1].IsObserved);
            Assert.True(animations.IsRunning);
        }

        // A table that really changed, which is what a tap and a BPM entry produce, still rebuilds.
        [AvaloniaFact]
        public void MakeStoryBoard_OnAChangedPeriodTable_StillRebuilds()
        {
            var animations = new BeatAnimations();
            animations.MakeStoryBoard(new[] { 500f, 1000f, 2000f });

            var built = animations.AnimatedDoubles.ToList();

            animations.MakeStoryBoard(new[] { 250f, 500f, 1000f });

            Assert.Equal(3, animations.AnimatedDoubles.Count);
            for (var i = 0; i < built.Count; i++)
                Assert.NotSame(built[i], animations.AnimatedDoubles[i]);
            Assert.Equal(250d, animations.AnimatedDoubles[0].Period);
        }

        // The whole point of the guard, seen from the beat modifier that a rebuild used to strand:
        // a multiply followed by a divide is back where it started, so the modifier has to be
        // stepping the very instance it began on rather than the same slot of a rebuilt storyboard.
        [AvaloniaFact]
        public void BeatRandomModulator_KeepsSteppingTheSameStoryboard_AcrossTempoClicks()
        {
            var provider = TestServiceProviderFactory.Create();
            var masterBeat = provider.GetRequiredService<MasterBeat>();

            // Built before the modifier is resolved, exactly as App does at startup, so the
            // modifier has a provider to resolve its animated double from.
            _ = new MasterBeatAnimationController(masterBeat);
            var modifier = provider.GetRequiredService<BeatRandomModulator>();

            var stepped = modifier.AnimatedDouble;
            Assert.NotNull(stepped);

            masterBeat.Multiply();
            masterBeat.Divide();

            Assert.Same(stepped, modifier.AnimatedDouble);
            Assert.True(((AnimatedDouble)modifier.AnimatedDouble).IsObserved);

            var before = modifier.BeatSteps.CurrentStepIndex;
            Pulse((AnimatedDouble)modifier.AnimatedDouble);

            Assert.Equal(before + 1, modifier.BeatSteps.CurrentStepIndex);
        }

        // MasterBeat re raises its own PropertyChanged for AnimatedDouble on every position change
        // of the instance it currently holds, which happens on every 16 ms clock tick. BeatRandomModulator
        // used to listen to that notification with no property filter, so every tick made it re
        // resolve and unconditionally reassign its AnimatedDouble property, which unsubscribes and
        // resubscribes PositionChanged even when the resolved instance never changed, driving the
        // observer count from 1 to 0 back to 1 on every single tick.
        [AvaloniaFact]
        public void BeatRandomModulator_DoesNotResubscribeItsAnimatedDouble_OnEveryPerTickPositionChange()
        {
            var provider = TestServiceProviderFactory.Create();
            var masterBeat = provider.GetRequiredService<MasterBeat>();

            _ = new MasterBeatAnimationController(masterBeat);
            var modifier = provider.GetRequiredService<BeatRandomModulator>();

            var animatedDouble = (AnimatedDouble)modifier.AnimatedDouble;
            Assert.NotNull(animatedDouble);
            Assert.True(animatedDouble.IsObserved);

            var observationStartedCount = 0;
            animatedDouble.ObservationStarted += (s, e) => observationStartedCount++;

            for (int i = 0; i < 20; i++)
                animatedDouble.AnimationPosition = i % 2 == 0 ? 0.2 : 0.8;

            Assert.Same(animatedDouble, modifier.AnimatedDouble);
            Assert.True(animatedDouble.IsObserved);
            // A resubscribe drops the count to zero and back to one, which fires ObservationStarted
            // again. Zero further firings after the initial subscribe means the count never dipped.
            Assert.Equal(0, observationStartedCount);
        }

        // One beat seen by a modifier: the position falls below the halfway mark and rises back
        // through it, which is where BeatRandomModulator advances its step.
        private static void Pulse(AnimatedDouble animatedDouble)
        {
            animatedDouble.AnimationPosition = 0.2;
            animatedDouble.AnimationPosition = 0.8;
        }
    }
}
