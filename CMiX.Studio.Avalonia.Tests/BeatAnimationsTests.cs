using System;
using System.ComponentModel;
using Avalonia.Headless.XUnit;
using CMiX.Studio.Avalonia.Animations;
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
        public void Clock_Starts_WhenABeatModifierListensForPulses()
        {
            var animations = new BeatAnimations();
            animations.MakeStoryBoard(new[] { 500f, 1000f, 2000f });

            EventHandler handler = (s, e) => { };
            animations.AnimatedDoubles[0].PositionChanged += handler;

            Assert.True(animations.IsRunning);
            Assert.True(animations.AnimatedDoubles[0].IsObserved);
        }
    }
}
